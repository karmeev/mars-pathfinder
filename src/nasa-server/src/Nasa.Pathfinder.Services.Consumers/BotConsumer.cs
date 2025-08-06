using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Consumers.Interfaces;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Consumers;

public class BotConsumer(
    IBotProcessorService processor,
    IWorldMapService worldMap) : IBotConsumer<MoveCommand>
{
    public async Task Consume(MoveCommand command, CancellationToken ct = default)
    {
        var funerals = await worldMap.GetFuneralsAsync(ct);
        var isTrap = funerals.Contains(command.DesiredPosition);
        
        var isAccessible = await worldMap.TryReachPosition(command.DesiredPosition, ct);
        if (isTrap && isAccessible)
        {
            var stand = new StandCommand(command.ClientId, command.Bot.Id, command.CorrelationId);
            processor.Publish(stand);
            return;
        }
        
        if (!isAccessible)
        {
            var lost = new DeadCommand(command.ClientId, command.Bot.Id, command.DesiredPosition,
                command.CorrelationId);
            processor.Publish(lost);
            return;
        }

        var walk = new WalkCommand(command.ClientId, command.Bot.Id, command.DesiredPosition,
            command.CorrelationId);
        processor.Publish(walk);
    }
}