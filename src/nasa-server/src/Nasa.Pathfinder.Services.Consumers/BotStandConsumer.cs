using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Services.Consumers.Interfaces;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Consumers;

public class BotStandConsumer(
    IMessageDecoderService messageDecoder,
    IOperatorStream stream) : IBotConsumer<StandCommand>
{
    public async Task Consume(StandCommand command, CancellationToken ct = default)
    {
        //get current position of bot
        var notificationText = messageDecoder.EncodeBotMessage(new Position(), false, true);
        var request = new SendMessageRequest(command.BotId, command.ClientId, notificationText, false);
        stream.SendMessage(request);
    }
}