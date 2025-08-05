using Nasa.Pathfinder.Hubs;

namespace Nasa.Pathfinder.Background;

public class StartMessageHubBackgroundTask(MessageHub hub): IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        hub.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}