using Nasa.Pathfinder.Domain.Bots;

namespace Nasa.Pathfinder.Domain.Interactions;

public interface IBotCommand
{
    public Guid CorrelationId { get; }
}

public record MoveCommand(
    string ClientId,
    Bot Bot,
    Position DesiredPosition,
    Guid CorrelationId = default) : IBotCommand;

public record WalkCommand(
    string ClientId,
    string BotId,
    Position DesiredPosition,
    Guid CorrelationId = default) : IBotCommand;

public record StandCommand(
    string ClientId,
    string BotId,
    Guid CorrelationId = default) : IBotCommand;

public record DeadCommand(
    string ClientId,
    string BotId,
    Position DesiredPosition,
    Guid CorrelationId = default) : IBotCommand;

public record InvalidCommand(
    string ClientId,
    string BotId,
    string Message,
    Guid CorrelationId = default) : IBotCommand;    