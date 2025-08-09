using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Data.Internal;

internal class GraveRepository(IDataContext context) : IGraveRepository
{
    public async Task AddNewGrave(Grave grave, CancellationToken ct = default)
    {
        var existedGrave = await context.GetAsync<Grave>(grave.Id, ct);
        if (existedGrave is not null)
        {
            DataException.ThrowIfIncorrectData("Grave already exists");
        }

        await context.PushAsync(grave, ct);
    }

    public async Task<IReadOnlyCollection<Grave>> GetGravesAsync(string mapId, CancellationToken ct = default)
    {
        var graves = await context.GetAllAsync<Grave>(ct);
        return graves.Where(f => f.MapId == mapId).ToList();
    }
}