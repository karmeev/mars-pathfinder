using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;

namespace Nasa.Pathfinder.Facades.Internal;

internal class MessageFacade : IMessageFacade
{
    public Task ReceiveMessageAsync(IMessage message)
    {
        throw new NotImplementedException();
    }
}