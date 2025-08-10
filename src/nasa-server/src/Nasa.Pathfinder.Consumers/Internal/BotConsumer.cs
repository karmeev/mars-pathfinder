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
        var project = await worldMap.ReachPosition(command.Bot.MapId, command.Bot.Position, command.OperatorCommands , ct);

        if (project is PositionNotChanged)
        {
            var stand = new StandCommand(command.ClientId, command.Bot.Id, project.Position,
                command.CorrelationId);
            processor.Publish(stand);
            return;
        }

        if (project is PositionOutOfMap outOfMap)
        {
            var lost = new DeadCommand(command.ClientId, command.Bot.Id, command.Bot.MapId,
                outOfMap.Previous,  command.CorrelationId);
            processor.Publish(lost);
            return;
        }

        var walk = new WalkCommand(command.ClientId, command.Bot.Id, project.Position,
            command.CorrelationId);
        processor.Publish(walk);
    }
}