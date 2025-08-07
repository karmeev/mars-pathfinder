using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Consumers.Contracts;

public interface IBotConsumer<T> where T: class, IBotCommand
{
    Task Consume(T command, CancellationToken ct = default);
}