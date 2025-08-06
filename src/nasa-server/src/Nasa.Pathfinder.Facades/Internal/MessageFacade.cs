using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Facades.Internal;

internal class MessageFacade(
    IBotRepository repository,
    IMessageDecoderService messageDecoder,
    IWorldMapService worldMap,
    IBotProcessorService processor) : IMessageFacade
{
    public async Task ReceiveMessageAsync(IMessage message, CancellationToken ct = default)
    {
        if (message is not OperatorMessage operatorMessage)
            throw new InvalidCastException();
        
        var commands = messageDecoder.DecodeOperatorMessage(operatorMessage.Text);
        
        var bot = await repository.GetAsync(operatorMessage.BotId, ct);

        var desiredPosition = worldMap.CalculateDesiredPosition(bot.Position, commands);
        
        var funerals = await worldMap.GetFuneralsAsync(ct);
        var isTrap = funerals.Contains(desiredPosition);
        
        await processor.RouteAsync(new MoveCommand(operatorMessage.ClientId, bot, desiredPosition, isTrap, 
            Guid.CreateVersion7()), ct);
    }
}