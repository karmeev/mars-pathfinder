using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotInvalidCommandConsumer(IOperatorProcessor processor) : IBotConsumer<InvalidCommand>
{
    public Task Consume(InvalidCommand command, CancellationToken ct = default)
    {
        var request = new SendMessageRequest(command.ClientId, command.BotId, command.Message, true,
            true);

        processor.SendMessage(request);

        return Task.CompletedTask;
    }
}