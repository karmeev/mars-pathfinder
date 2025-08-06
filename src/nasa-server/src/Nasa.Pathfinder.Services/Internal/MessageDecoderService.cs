using Nasa.Pathfinder.Domain.Interactions;
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

    public string EncodeBotMessage(Position position, bool isLost, bool isSave)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyList<IOperatorCommand> DecodeOperatorMessage(string text)
    {
        if (string.IsNullOrWhiteSpace(text) || text.Length > 100)
        {
            throw new InvalidOperationException("Message is too long or too short for command!");
        }
        
        var commands = new List<IOperatorCommand>();
        var symbols = text.Select(c => c.ToString()).ToArray();
        foreach (var input in symbols)
        {
            if (_commands.TryGetValue(input, out var command))
            {
                commands.Add(command);
            }
            else
            {
                commands.Add(new UnknownCommand());
            }
        }
        
        return commands;
    }
}