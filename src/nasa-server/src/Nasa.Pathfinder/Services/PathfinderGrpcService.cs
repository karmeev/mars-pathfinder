using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;
using Nasa.Pathfinder.Hubs;
using Nasa.Pathfinder.Types;
using Pathfinder.Messages;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService(
    ILogger<PathfinderGrpcService> logger,
    MessageHub hub,
    IBotFacade botFacade,
    IMessageFacade messageFacade) : PathfinderService.PathfinderServiceBase
{
    public override async Task<PingResponse> Ping(Empty request, ServerCallContext context)
    {
        await Task.CompletedTask;
        return new PingResponse
        {
            IsSuccessful = true
        };
    }

    public override async Task<GetBotsResponse> GetBots(Empty request, ServerCallContext context)
    {
        var result = await botFacade.GetBotsAsync(context.CancellationToken);

        var response = Response.MapToGetBotsResponse(result);
        return response;
    }

    public override async Task<SelectBotResponse> SelectBot(SelectBotRequest request, ServerCallContext context)
    {
        try
        {
            var bot = await botFacade.SelectBotAsync(request.BotId, context.CancellationToken);
            return Response.MapToSelectBotResponse(bot);
        }
        catch (BotAlreadyAcquiredException e)
        {
            return Response.InvalidArgument<SelectBotResponse>("Bot already acquired");
        }
    }

    public override async Task<ResetBotResponse> ResetBot(ResetBotRequest request, ServerCallContext context)
    {
        try
        {
            var bot = await botFacade.ResetBotAsync(request.BotId, context.CancellationToken);
            return Response.MapToResetBotResponse(bot);
        }
        catch (BotAlreadyReleasedException e)
        {
            return Response.InvalidArgument<ResetBotResponse>("Bot already released");
        }
    }

    public override async Task SendMessage(IAsyncStreamReader<SendMessageRequest> requestStream,
        IServerStreamWriter<SendMessageResponse> responseStream, ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var clientId = headers.GetValue("clientId");

        try
        {
            hub.Connect(clientId, responseStream);

            await foreach (var message in requestStream.ReadAllAsync())
            {
                var operatorMessage = new OperatorMessage
                {
                    ClientId = clientId,
                    BotId = message.BotId,
                    Text = message.Message
                };
                await messageFacade.ReceiveMessageAsync(operatorMessage, context.CancellationToken);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            logger.LogError("completed; Client {clientId} disconnected intentionally.", clientId);
            hub.Disconnect(clientId);
        }
        catch (IOException ex)
        {
            logger.LogWarning("completed; Client {clientId} stream aborted: {exceptionMessage}", clientId, ex.Message);
            hub.Disconnect(clientId);
        }
        catch (Exception ex)
        {
            logger.LogError("completed; Client {clientId} stream aborted: {exceptionMessage}", clientId, ex.Message);
            hub.Disconnect(clientId);
        }
        finally
        {
            logger.LogInformation("completed; Message received; Client: {clientId}", clientId);
        }
    }
}