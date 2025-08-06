using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Consumers.Interfaces;

namespace Nasa.Pathfinder.Services.Consumers;

public class BotInvalidCommandConsumer : IBotConsumer<InvalidCommand>
{
    public async Task Consume(InvalidCommand command, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}