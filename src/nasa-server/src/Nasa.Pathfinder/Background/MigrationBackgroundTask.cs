using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Background;

public class MigrationBackgroundTask(IMemoryDataContext context) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await context.PushAsync(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-1",
            Status = BotStatus.Available,
            Position = new Position
            {
                X = 1,
                Y = 1,
                Direction = Direction.N
            }
        }, cancellationToken);

        await context.PushAsync(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-2",
            Status = BotStatus.Available,
            Position = new Position
            {
                X = 10,
                Y = 6,
                Direction = Direction.N
            }
        }, cancellationToken);

        await context.PushAsync(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-3",
            Status = BotStatus.Available,
            Position = new Position
            {
                X = 5,
                Y = 5,
                Direction = Direction.N
            }
        }, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}