using Nasa.Pathfinder.Domain.Entities.World;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IMapRepository
{
    Task<MapInfo?> TryGetAsync(string id, CancellationToken ct = default);
    Task<MapInfo> AddAsync(MapInfo map, CancellationToken ct = default);
}