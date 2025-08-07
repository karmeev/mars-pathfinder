using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotDeadWalkerConsumer(
    IWorldMapService worldMap,
    IMessageDecoderService messageDecoder,
    IBotRepository repository,
    IOperatorProcessor processor) : IBotConsumer<DeadCommand>
{
    public async Task Consume(DeadCommand command, CancellationToken ct = default)
    {
        await repository.ChangeBotStatusAsync(command.BotId, BotStatus.Dead, ct);
        await worldMap.ChangeBotPositionAsync(command.DesiredPosition, ct);
        var notificationText = messageDecoder.EncodeBotMessage(command.DesiredPosition, true, false);
        var request = new SendMessageRequest(command.BotId, command.ClientId, notificationText, true, 
            false);
        processor.SendMessage(request);
    }
}