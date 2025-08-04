using System.Threading.Channels;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;

namespace Nasa.Pathfinder.Infrastructure.Internal.Grpc;

internal class OperatorStream(ChannelWriter<SendMessageRequest> writer) : IOperatorStream
{
    public void SendMessage(SendMessageRequest message)
    {
        Task.Run(() => { writer.TryWrite(message); });
    }
}