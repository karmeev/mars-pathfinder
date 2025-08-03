using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Pathfinder.Messages;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService : PathfinderService.PathfinderServiceBase
{
    //private static readonly ConcurrentDictionary<string, IServerStreamWriter<MoveResponse>> ClientStreams = new();

    public override async Task<PingResponse> Ping(Empty request, ServerCallContext context)
    {
        var headers = context.RequestHeaders;
        var traceId = headers.GetValue("traceId") ?? Guid.NewGuid().ToString();
        
        var responseHeaders = new Metadata
        {
            { "traceId", traceId }
        };
        
        await context.WriteResponseHeadersAsync(responseHeaders);
        
        return new PingResponse
        {
            IsSuccessful = true
        };
    }

    public override async Task Move(IAsyncStreamReader<MoveRequest> requestStream, 
        IServerStreamWriter<MoveResponse> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            Console.WriteLine($"[{Activity.Current?.TraceId}] received message from client; botId: {message.BotId}");
        }
    }
}