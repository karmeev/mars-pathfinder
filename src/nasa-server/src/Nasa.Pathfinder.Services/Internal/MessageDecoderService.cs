using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class MessageDecoderService : IMessageDecoderService
{
    public string EncodeBotMessage(Position position, bool isLost, bool isSave)
    {
        throw new NotImplementedException();
    }

    public BotCommand DecodeOperatorMessage(string text)
    {
        throw new NotImplementedException();
    }
}