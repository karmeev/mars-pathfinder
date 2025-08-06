using Nasa.Pathfinder.Hubs;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Background;

public class StartConsumersBackgroundTask(
    IBotProcessorService processor,
    MessageHub hub) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        hub.Start();
        processor.RunConsumers();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}