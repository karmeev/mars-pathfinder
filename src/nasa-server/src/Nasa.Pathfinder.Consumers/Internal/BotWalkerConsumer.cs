using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotWalkerConsumer(
    IMessageDecoderService messageDecoder,
    IBotRepository repository,
    IOperatorProcessor processor) : IBotConsumer<WalkCommand>
{
    public async Task Consume(WalkCommand command, CancellationToken ct = default)
    {
        await repository.ChangeBotPositionAsync(command.BotId, command.DesiredPosition, ct);
        var notificationText = messageDecoder.EncodeBotMessage(command.DesiredPosition, false);
        var request = new SendMessageRequest(command.BotId, command.ClientId, notificationText, false,
            false);
        processor.SendMessage(request);
    }
}