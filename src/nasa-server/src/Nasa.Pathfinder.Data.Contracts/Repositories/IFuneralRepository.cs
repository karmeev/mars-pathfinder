using Nasa.Pathfinder.Domain.Entities.World;

namespace Nasa.Pathfinder.Data.Contracts.Repositories;

public interface IFuneralRepository
{
    Task AddNewFuneral(Funeral funeral, CancellationToken ct = default);
    Task<IReadOnlyCollection<Funeral>> GetFuneralsAsync(string mapId, CancellationToken ct = default);
}