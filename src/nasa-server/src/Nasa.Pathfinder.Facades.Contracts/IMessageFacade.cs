using Nasa.Pathfinder.Domain.Messages;

namespace Nasa.Pathfinder.Facades.Contracts;

public interface IMessageFacade
{
    Task ReceiveMessageAsync(IMessage message);
}