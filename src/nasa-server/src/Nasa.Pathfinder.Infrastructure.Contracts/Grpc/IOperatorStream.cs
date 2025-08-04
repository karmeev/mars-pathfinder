using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;

namespace Nasa.Pathfinder.Infrastructure.Contracts.Grpc;

public interface IOperatorStream
{
    void SendMessage(SendMessageRequest message);
}