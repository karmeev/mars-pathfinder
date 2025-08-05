using Nasa.Pathfinder.Domain.Bots;

namespace Nasa.Pathfinder.Facades.Contracts;

public interface IBotFacade
{
    Task<IEnumerable<Bot>> GetBotsAsync(CancellationToken ct = default);
    Task<Bot> SelectBotAsync(string botId, CancellationToken ct = default);
    Task<Bot> ResetBotAsync(string botId, CancellationToken ct = default);
}