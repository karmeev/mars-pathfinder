namespace Nasa.Pathfinder.Domain.Interactions;

public record BotCommand(IReadOnlyList<IOperatorCommand> OperatorCommands);

public interface IOperatorCommand { }

public record UnknownCommand : IOperatorCommand { }

public record MoveRight : IOperatorCommand {}

public record MoveLeft : IOperatorCommand {}

public record MoveFront : IOperatorCommand {}