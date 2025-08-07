using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
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
        //get current position of bot
        var notificationText = messageDecoder.EncodeBotMessage(new Position(), false, true);
        var request = new SendMessageRequest(command.BotId, command.ClientId, notificationText, false, 
            false);
        processor.SendMessage(request);
    }
}