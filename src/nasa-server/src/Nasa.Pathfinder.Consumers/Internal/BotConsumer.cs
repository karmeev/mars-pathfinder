using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotConsumer(
    IBotProcessor processor,
    IWorldMapService worldMap) : IBotConsumer<MoveCommand>
{
    public async Task Consume(MoveCommand command, CancellationToken ct = default)
    {
        var project = await worldMap.TryReachPosition(command.Bot.MapId, command.DesiredPosition, ct);

        if (project is PositionNotChanged)
        {
            var stand = new StandCommand(command.ClientId, command.Bot.Id, command.Bot.Position, command.CorrelationId);
            processor.Publish(stand);
            return;
        }

        if (project is PositionOutOfMap)
        {
            var lost = new DeadCommand(command.ClientId, command.Bot.Id, command.DesiredPosition, command.Bot.MapId,
                command.Bot.LastWords, command.CorrelationId);
            processor.Publish(lost);
            return;
        }

        var walk = new WalkCommand(command.ClientId, command.Bot.Id, command.DesiredPosition,
            command.CorrelationId);
        processor.Publish(walk);
    }
}