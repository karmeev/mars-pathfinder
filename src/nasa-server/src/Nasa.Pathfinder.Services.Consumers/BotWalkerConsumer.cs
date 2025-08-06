using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Services.Consumers.Interfaces;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Consumers;

public class BotWalkerConsumer(
    IWorldMapService worldMap,
    IMessageDecoderService messageDecoder,
    IOperatorStream stream) : IBotConsumer<WalkCommand>
{
    public async Task Consume(WalkCommand command, CancellationToken ct = default)
    {
        await worldMap.ChangeBotPositionAsync(command.DesiredPosition, ct);
        var notificationText = messageDecoder.EncodeBotMessage(command.DesiredPosition, false, false);
        var request = new SendMessageRequest(command.BotId, command.ClientId, notificationText, false);
        stream.SendMessage(request);
    }
}