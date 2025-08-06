using Grpc.Core;
using Pathfinder.Proto;

namespace Nasa.Pathfinder.Services;

public class GrpcService : PathfinderService.PathfinderServiceBase
{
    protected T InvalidArgument<T>(string message)
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, message));
    }
}