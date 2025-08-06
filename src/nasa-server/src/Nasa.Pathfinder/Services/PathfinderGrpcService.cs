using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;
using Nasa.Pathfinder.Hubs;
using Pathfinder.Messages;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService(
    ILogger<PathfinderGrpcService> logger,
    MessageHub hub,
    IBotFacade botFacade, 
    IMessageFacade messageFacade) : GrpcService
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
        
        var response = new GetBotsResponse();
        foreach (var bot in result)
        {
            response.Bots.Add(new Bot
            {
                Id = bot.Id,
                Name = bot.Name,
                Status = bot.Status.ToString(),
            });
        }
        
        logger.LogInformation("GetBots: {GetBots}", response.Bots.Select(b => b.Id).ToList());

        return response;
    }

    public override async Task<SelectBotResponse> SelectBot(SelectBotRequest request, ServerCallContext context)
    {
        try
        {
            var bot = await botFacade.SelectBotAsync(request.BotId, context.CancellationToken);
            return new SelectBotResponse
            {
                Bot = new Bot
                {
                    Id = bot.Id,
                    Name = bot.Name,
                    Status = bot.Status.ToString(),
                },
            };
        }
        catch (BotAlreadyAcquiredException e)
        {
            return InvalidArgument<SelectBotResponse>("Bot already acquired");
        }
    }

    public override async Task<ResetBotResponse> ResetBot(ResetBotRequest request, ServerCallContext context)
    {
        try
        {
            var bot = await botFacade.ResetBotAsync(request.BotId, context.CancellationToken);
            var response = new ResetBotResponse
            {
                Bot = new Bot
                {
                    Id = bot.Id,
                    Name = bot.Name,
                    Status = bot.Status.ToString(),
                },
            };
            return response;
        }
        catch (BotAlreadyReleasedException e)
        {
            return InvalidArgument<ResetBotResponse>("Bot already released");
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
        }
        catch (IOException ex)
        {
            logger.LogWarning("completed; Client {clientId} stream aborted: {exceptionMessage}", clientId, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError("completed; Client {clientId} stream aborted: {exceptionMessage}", clientId, ex.Message);
        }
        finally
        {
            logger.LogInformation("completed; Client {clientId} cleanup done.", clientId);
            hub.Disconnect(clientId);
        }
    }
}