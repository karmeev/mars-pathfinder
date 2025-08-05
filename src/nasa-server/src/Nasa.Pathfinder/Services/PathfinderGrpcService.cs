using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;
using Nasa.Pathfinder.Hubs;
using Nasa.Pathfinder.Tmp;
using Pathfinder.Messages;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService(
    BotStorage storage,
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
            return BadRequest<SelectBotResponse>("Bot already acquired");
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
            return BadRequest<ResetBotResponse>("Bot already released");
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
            Console.WriteLine($"Client {clientId} disconnected intentionally.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[WARN] Client {clientId} stream aborted: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Client {clientId} error: {ex}");
        }
        finally
        {
            Console.WriteLine($"Client {clientId} cleanup done.");
            hub.Disconnect(clientId);
        }
    }
}