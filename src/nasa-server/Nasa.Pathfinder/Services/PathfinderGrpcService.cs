using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Tmp;
using Pathfinder.Messages;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService(BotStorage storage, IBotFacade botFacade) : PathfinderService.PathfinderServiceBase
{
    //private static readonly ConcurrentDictionary<string, IServerStreamWriter<MoveResponse>> ClientStreams = new();

    public override async Task<PingResponse> Ping(Empty request, ServerCallContext context)
    {
        await MakeHeader(context);
        
        return new PingResponse
        {
            IsSuccessful = true
        };
    }

    public override async Task<GetBotsResponse> GetBots(Empty request, ServerCallContext context)
    {
        await MakeHeader(context);
        
        var result = await botFacade.GetBotsAsync(context.CancellationToken);
        
        var response = new GetBotsResponse();
        response.Bots.Add(storage.Bots);
        return response;
    }

    public override async Task<SelectBotResponse> SelectBot(SelectBotRequest request, ServerCallContext context)
    {
        await MakeHeader(context);

        var result = await botFacade.SelectBotAsync(request.BotId, context.CancellationToken);
        
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
        await MakeHeader(context);
        
        var result = await botFacade.ResetBotAsync(request.BotId, context.CancellationToken);
        
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
        var traceId = headers.GetValue("traceId") ?? Guid.NewGuid().ToString();
        
        try
        {
            await foreach (var message in requestStream.ReadAllAsync())
            {
                Console.WriteLine($"[{Activity.Current?.TraceId}] received message from client; botId: {message.BotId}");
                
                await NotifyClient(message, responseStream);
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            Console.WriteLine($"Client {traceId} disconnected intentionally.");
        }
        catch (IOException ex)
        {
            Console.WriteLine($"[WARN] Client {traceId} stream aborted: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Client {traceId} error: {ex}");
        }
        finally
        {
            Console.WriteLine($"Client {traceId} cleanup done.");
            // Remove from client registry if needed
        }
    }

    private async Task MakeHeader(ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var traceId = headers.GetValue("traceId") ?? Guid.NewGuid().ToString();
        
        var responseHeaders = new Metadata
        {
            { "traceId", traceId }
        };
        
        await context.WriteResponseHeadersAsync(responseHeaders);
    }

    private async Task NotifyClient(SendMessageRequest message, IServerStreamWriter<SendMessageResponse> responseStream)
    {
        var reply = new SendMessageResponse
        {
            BotId = message.BotId,
            Message = $"Received message from Bot {message.BotId}",
            IsLost = false
        };

        await responseStream.WriteAsync(reply);
    }
}