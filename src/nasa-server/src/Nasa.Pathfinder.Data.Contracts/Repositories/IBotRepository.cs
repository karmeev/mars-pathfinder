using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IBotRepository
{
    Task AddRangeAsync(IEnumerable<Bot> bots, CancellationToken ct = default);
    Task<Bot?> TryGetAsync(string botId, CancellationToken ct = default);
    Task<IEnumerable<Bot>> GetBotsAsync(string mapId, CancellationToken ct = default);
    Task<Bot?> ChangeBotStatusAsync(string botId, BotStatus status, CancellationToken ct = default);
    Task ChangeBotPositionAsync(string botId, Position position, CancellationToken ct = default);
}