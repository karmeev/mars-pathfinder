using Nasa.Pathfinder.Domain.Entities.World;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IGraveRepository
{
    Task AddNewGrave(Grave grave, CancellationToken ct = default);
    Task<IReadOnlyCollection<Grave>> GetGravesAsync(string mapId, CancellationToken ct = default);
}