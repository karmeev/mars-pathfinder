using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;

namespace Nasa.Pathfinder.Infrastructure.Contracts.Processors;

public interface IOperatorProcessor
{
    void SendMessage(SendMessageRequest message);
}