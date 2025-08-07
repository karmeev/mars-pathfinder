using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Data.Internal;

internal class FuneralRepository(IDataContext context) : IFuneralRepository
{
    public async Task AddNewFuneral(Funeral funeral, CancellationToken ct = default)
    {
        var existedFuneral = await context.GetAsync<Funeral>(funeral.Id, ct);
        if (existedFuneral is not null)
        {
            DataException.ThrowIfIncorrectData("Funeral already exists");
        }

        await context.PushAsync(funeral, ct);
    }

    public async Task<IReadOnlyCollection<Funeral>> GetFuneralsAsync(string mapId, CancellationToken ct = default)
    {
        var funerals = await context.GetAllAsync<Funeral>(ct);
        return funerals.Where(f => f.MapId == mapId).ToList();
    }
}