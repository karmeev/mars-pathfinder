using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Background;

public class MigrationBackgroundTask(
    IMemoryDataContext context) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mapId = Guid.CreateVersion7().ToString();
        var mapInfo = new MapInfo
        {
            Id = mapId,
            ETag = Guid.NewGuid(),
            SizeX = 50,
            SizeY = 50,
        };
        
        await context.PushAsync(mapInfo, cancellationToken);
        
        await context.PushAsync(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-1",
            Status = BotStatus.Available,
            MapId = mapId,
            LastWords = "beep-bep-bep-beee.",
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
            MapId = mapId,
            LastWords = "There is only the Emperor, and he is our shield and protector.",
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
            MapId = mapId,
            LastWords = "Blessed is the mind too small for doubt.",
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