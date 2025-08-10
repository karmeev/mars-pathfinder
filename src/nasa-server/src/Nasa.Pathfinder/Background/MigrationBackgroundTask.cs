using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Background;

public class MigrationBackgroundTask(
    IBotRepository botRepository,
    IMemoryDataContext context) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var mapId = "1";
        var mapInfo = new MapInfo
        {
            Id = mapId,
            ETag = Guid.NewGuid(),
            SizeX = 5,
            SizeY = 3,
        };
        
        await context.PushAsync(mapInfo, cancellationToken);
        
        var bots = new List<Bot>();
        bots.Add(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-1",
            Status = BotStatus.Available,
            MapId = mapId,
            Position = new Position
            {
                X = 1,
                Y = 1,
                Direction = Direction.E
            }
        });
        bots.Add(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-2",
            Status = BotStatus.Available,
            MapId = mapId,
            Position = new Position
            {
                X = 3,
                Y = 2,
                Direction = Direction.N
            }
        });
        bots.Add(new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = "bot-3",
            Status = BotStatus.Available,
            MapId = mapId,
            Position = new Position
            {
                X = 0,
                Y = 3,
                Direction = Direction.W
            }
        });
        
        await botRepository.AddRangeAsync(bots, cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}