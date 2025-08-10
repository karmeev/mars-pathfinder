using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Data.Internal;

internal class MapRepository(IDataContext context) : IMapRepository
{
    public async Task<MapInfo?> TryGetAsync(string id, CancellationToken ct = default)
    {
        return await context.GetAsync<MapInfo>(id, ct);
    }
    
    public async Task<MapInfo> AddAsync(MapInfo map, CancellationToken ct = default)
    {
        map.Id = Guid.NewGuid().ToString();
        map.ETag = Guid.NewGuid();
        
        await context.PushAsync(map, ct);
        return map;
    }
}