using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Infrastructure.Contracts.Processors;

public interface IBotProcessor
{
    void RunConsumers();
    void Publish(IBotCommand command);
}