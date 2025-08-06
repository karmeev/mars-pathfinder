using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IMessageDecoderService
{
    string EncodeBotMessage(Position position, bool isLost, bool isSave);
    IReadOnlyList<IOperatorCommand> DecodeOperatorMessage(string text);
}