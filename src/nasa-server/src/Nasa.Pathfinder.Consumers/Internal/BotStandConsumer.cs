using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotStandConsumer(
    IMessageDecoderService messageDecoder,
    IOperatorProcessor processor) : IBotConsumer<StandCommand>
{
    public async Task Consume(StandCommand command, CancellationToken ct = default)
    {
        var notificationText = messageDecoder.EncodeBotMessage(command.CurrentPosition, false);
        var request = new SendMessageRequest(command.ClientId, command.BotId, notificationText, false,
            false, string.Empty);
        processor.SendMessage(request);
    }
}