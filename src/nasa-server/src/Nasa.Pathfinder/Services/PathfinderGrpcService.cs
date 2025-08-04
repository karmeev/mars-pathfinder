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
    //private static readonly ConcurrentDictionary<string, IServerStreamWriter<MoveResponse>> ClientStreams = new();

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
        response.Bots.Add(storage.Bots);
        return response;
    }

    public override async Task<SelectBotResponse> SelectBot(SelectBotRequest request, ServerCallContext context)
    {
        try
        {
            var result = await botFacade.SelectBotAsync(request.BotId, context.CancellationToken);
        }
        catch (BotAlreadyAcquiredException e)
        {
            return BadRequest<SelectBotResponse>("Bot already acquired");
        }
        
        var bot = storage.Bots.FirstOrDefault(x => x.Id == request.BotId);
        
        var response = new SelectBotResponse
        {
            Bot = new Bot
            {
                Id = bot.Id,
                Name = bot.Name,
                Status = bot.Status,
            },
        };
        
        storage.Bots.Remove(bot);
        bot.Status = "Acquired";
        storage.Bots.Add(bot);
        
        return response;
    }

    public override async Task<ResetBotResponse> ResetBot(ResetBotRequest request, ServerCallContext context)
    {
        try
        {
            var result = await botFacade.ResetBotAsync(request.BotId, context.CancellationToken);
        }
        catch (BotAlreadyReleasedException e)
        {
            return BadRequest<ResetBotResponse>("Bot already acquired");
        }
        
        storage.Bots.Find(x => x.Id == request.BotId).Status = "Available";
        
        var response = new ResetBotResponse
        {
            Bot = storage.Bots.FirstOrDefault(x => x.Id == request.BotId)
        };
        return response;
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
                await messageFacade.ReceiveMessageAsync(operatorMessage);
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