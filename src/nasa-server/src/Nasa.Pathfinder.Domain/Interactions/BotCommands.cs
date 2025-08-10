using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Domain.Interactions;

public interface IBotCommand
{
    public Guid CorrelationId { get; }
}

public record MoveCommand(
    string ClientId,
    Bot Bot,
    Stack<IOperatorCommand> OperatorCommands,
    Guid CorrelationId = default) : IBotCommand;

public record WalkCommand(
    string ClientId,
    string BotId,
    Position CurrentPosition,
    Guid CorrelationId = default) : IBotCommand;

public record StandCommand(
    string ClientId,
    string BotId,
    Position CurrentPosition,
    Guid CorrelationId = default) : IBotCommand;

public record DeadCommand(
    string ClientId,
    string BotId,
    string MapId,
    string LastWords,
    Position CurrentPosition,
    Guid CorrelationId = default) : IBotCommand;

public record InvalidCommand(
    string ClientId,
    string BotId,
    string Message,
    Guid CorrelationId = default) : IBotCommand;