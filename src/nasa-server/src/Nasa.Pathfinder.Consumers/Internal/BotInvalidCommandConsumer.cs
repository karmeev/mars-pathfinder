using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Consumers.Internal;

internal class BotInvalidCommandConsumer : IBotConsumer<InvalidCommand>
{
    public async Task Consume(InvalidCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}