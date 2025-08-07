using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IBotRepository
{
    Task<Bot> GetAsync(string botId, CancellationToken ct = default);
    Task<IEnumerable<Bot>> GetBotsAsync(CancellationToken ct = default);
    Task<Bot> ChangeBotStatusAsync(string botId, BotStatus status, CancellationToken ct = default);
    Task ChangeBotPositionAsync(string botId, Position position, CancellationToken ct = default);
}