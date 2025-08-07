using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotDeadWalkerConsumer(
    IMessageDecoderService messageDecoder,
    IBotRepository botRepository,
    IFuneralRepository funeralRepository,
    IOperatorProcessor processor) : IBotConsumer<DeadCommand>
{
    public async Task Consume(DeadCommand command, CancellationToken ct = default)
    {
        await botRepository.ChangeBotStatusAsync(command.BotId, BotStatus.Dead, ct);
        await botRepository.ChangeBotPositionAsync(command.BotId, command.DesiredPosition, ct);

        var funeral = new Funeral
        {
            Id = Guid.CreateVersion7().ToString(),
            ETag = Guid.NewGuid(),
            Value = command.DesiredPosition,
            MapId = command.MapId,
        };
        await funeralRepository.AddNewFuneral(funeral, ct);
        var notificationText = messageDecoder.EncodeBotMessage(command.DesiredPosition, true);
        var request = new SendMessageRequest(command.ClientId, command.BotId, notificationText, true,
            false, command.LastWords);
        processor.SendMessage(request);
    }
}