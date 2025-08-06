using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IBotProcessorService
{
    void RunConsumers();
    Task RouteAsync(IBotCommand command, CancellationToken ct = default);
}