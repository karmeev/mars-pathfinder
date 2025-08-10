using System.Numerics;
using Nasa.Pathfinder.Domain.Entities.Bots;

namespace Nasa.Pathfinder.Facades.Contracts;

public interface IBotFacade
{
    Task<string> CreateWorldAsync(Vector2 coordinates, IEnumerable<Bot> bots, CancellationToken ct = default);
    Task<IEnumerable<Bot>> GetBotsAsync(string mapId, CancellationToken ct = default);
    Task<Bot?> SelectBotAsync(string botId, CancellationToken ct = default);
    Task<Bot?> ResetBotAsync(string botId, CancellationToken ct = default);
}