using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;

namespace Nasa.Pathfinder.Facades.Internal;

internal class MessageFacade : IMessageFacade
{
    public async Task ReceiveMessageAsync(IMessage message)
    {
        await Task.CompletedTask;
    }
}