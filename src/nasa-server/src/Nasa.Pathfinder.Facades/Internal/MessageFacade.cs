using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Facades.Internal;

internal class MessageFacade(
    IBotRepository repository,
    IMessageDecoderService messageDecoder,
    IWorldMapService worldMap,
    IBotProcessor processor) : IMessageFacade
{
    public async Task ReceiveMessageAsync(IMessage message, CancellationToken ct = default)
    {
        var correlationId = Guid.CreateVersion7();

        if (message is not OperatorMessage operatorMessage)
            throw new InvalidCastException();

        Stack<IOperatorCommand> commands;

        try
        {
            commands = messageDecoder.DecodeOperatorMessage(operatorMessage.Text);
            if (commands.Any(x => x is UnknownCommand))
                throw new InvalidOperationException("Message has a unknown command!");
        }
        catch (InvalidOperationException ex)
        {
            processor.Publish(new InvalidCommand(operatorMessage.ClientId, operatorMessage.BotId, ex.Message,
                correlationId));
            return;
        }

        var bot = await repository.GetAsync(operatorMessage.BotId, ct);

        var desiredPosition = worldMap.CalculateDesiredPosition(bot.Position, commands);

        processor.Publish(new MoveCommand(operatorMessage.ClientId, bot, desiredPosition, correlationId));
    }
}