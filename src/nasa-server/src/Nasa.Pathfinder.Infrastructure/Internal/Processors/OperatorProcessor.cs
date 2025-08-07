using System.Threading.Channels;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;

namespace Nasa.Pathfinder.Infrastructure.Internal.Processors;

internal class OperatorProcessor(ChannelWriter<SendMessageRequest> writer) : IOperatorProcessor
{
    public void SendMessage(SendMessageRequest message)
    {
        Task.Run(() => { writer.TryWrite(message); });
    }
}