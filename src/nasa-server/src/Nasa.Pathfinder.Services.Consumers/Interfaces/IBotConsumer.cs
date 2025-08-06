using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Consumers.Interfaces;

public interface IBotConsumer<T> where T: class, IBotCommand
{
    Task Consume(T command, CancellationToken ct = default);
}