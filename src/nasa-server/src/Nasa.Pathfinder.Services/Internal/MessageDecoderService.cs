using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class MessageDecoderService : IMessageDecoderService
{
    private readonly Dictionary<string, IOperatorCommand> _commands = new()
    {
        { OperatorCommand.GetShorthand<MoveFront>(), new MoveFront() },
        { OperatorCommand.GetShorthand<MoveRight>(), new MoveRight() },
        { OperatorCommand.GetShorthand<MoveLeft>(), new MoveLeft() }
    };

    public string EncodeBotMessage(Position position, bool isLost)
    {
        if (isLost) return $"{position.X} {position.Y} {position.Direction} LOST";
        return $"{position.X} {position.Y} {position.Direction}";
    }

    public Stack<IOperatorCommand> DecodeOperatorMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 100)
            throw new InvalidOperationException("Message is too long or too short for command!");

        var symbols = text.Reverse().Select(c => c.ToString()).ToArray();

        var commands = new Stack<IOperatorCommand>();
        foreach (var input in symbols)
            if (_commands.TryGetValue(input, out var command))
                commands.Push(command);
            else
                commands.Push(new UnknownCommand());


        return commands;
    }
}