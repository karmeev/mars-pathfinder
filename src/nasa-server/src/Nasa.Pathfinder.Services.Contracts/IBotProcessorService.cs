using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IBotProcessorService
{
    void RunConsumers();
    void Publish(IBotCommand command);
}