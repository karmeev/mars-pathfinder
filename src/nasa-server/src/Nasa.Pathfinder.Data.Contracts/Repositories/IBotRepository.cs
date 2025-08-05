using Nasa.Pathfinder.Domain.Bots;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IBotRepository
{
    Task<Bot> GetAsync(string botId, CancellationToken ct = default);
    Task<IEnumerable<Bot>> GetBotsAsync(CancellationToken ct = default);
    Task<Bot> SelectBotAsync(string botId, CancellationToken ct = default);
    Task<Bot> ResetBotAsync(string botId, CancellationToken ct = default);
}