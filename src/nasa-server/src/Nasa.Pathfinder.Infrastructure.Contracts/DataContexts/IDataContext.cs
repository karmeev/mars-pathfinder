using ErrorOr;
using Nasa.Pathfinder.Domain.Core;

namespace Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

public interface IDataContext
{
    Task PushAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity;
    Task<ErrorOr<T>> UpdateAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity;
    Task<T> GetAsync<T>(string id, CancellationToken ct = default) where T : class, IEntity;
    Task<List<T>> GetAllAsync<T>(CancellationToken ct = default);
}