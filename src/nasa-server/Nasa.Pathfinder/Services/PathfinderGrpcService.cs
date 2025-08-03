using System.Diagnostics;
using Grpc.Core;
using Pathfinder.Models;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class PathfinderGrpcService : PathfinderService.PathfinderServiceBase
{
    //private static readonly ConcurrentDictionary<string, IServerStreamWriter<MoveResponse>> ClientStreams = new();

    public override async Task Move(IAsyncStreamReader<MoveRequest> requestStream, 
        IServerStreamWriter<MoveResponse> responseStream, ServerCallContext context)
    {
        await foreach (var message in requestStream.ReadAllAsync())
        {
            Console.WriteLine($"[{Activity.Current?.TraceId}] received message from client; botId: {message.BotId}");
        }
    }
}