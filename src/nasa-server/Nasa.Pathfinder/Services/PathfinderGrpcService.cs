using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Pathfinder.Tmp;
using Pathfinder.Messages;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService(BotStorage storage) : PathfinderService.PathfinderServiceBase
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
        
        var response = new GetBotsResponse();
        response.Bots.Add(storage.Bots);
        return response;
    }

    public override async Task<SelectBotResponse> SelectBot(SelectBotRequest request, ServerCallContext context)
    {
        await MakeHeader(context);

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
        
        storage.Bots.Find(x => x.Id == request.BotId).Status = "Available";
        
        var response = new ResetBotResponse
        {
            Bot = storage.Bots.FirstOrDefault(x => x.Id == request.BotId)
        };
        return response;
    }

    public override async Task Move(IAsyncStreamReader<MoveRequest> requestStream, 
        IServerStreamWriter<MoveResponse> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            Console.WriteLine($"[{Activity.Current?.TraceId}] received message from client; botId: {message.BotId}");
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
}