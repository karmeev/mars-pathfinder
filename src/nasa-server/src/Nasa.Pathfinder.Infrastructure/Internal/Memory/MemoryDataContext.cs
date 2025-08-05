using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using Nasa.Pathfinder.Domain.Core;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;
using Nasa.Pathfinder.Infrastructure.Contracts.Exceptions;

namespace Nasa.Pathfinder.Infrastructure.Internal.Memory;

internal class MemoryDataContext(IMemoryCache cache) : IMemoryDataContext
{
    private readonly HashSet<string> _keys = [];
    private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.NoRecursion);

    public async Task PushAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity
    {
        await Push(entry);
    }
    
    private ValueTask Push<T>(T entry) where T : class, IEntity
    {
        var id = GetInternalId<T>(entry.Id);
        cache.Set(id, entry);
        _keys.Add(id);
        return ValueTask.CompletedTask;
    }

    public Task<ErrorOr<T>> UpdateAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity
    {
        _lock.EnterWriteLock();
        var result = Update(entry);
        _lock.ExitWriteLock();
        
        return Task.FromResult(result);
    }
    
    private ErrorOr<T> Update<T>(T entry) where T : class, IEntity
    {
        var entity = Get<T>(entry.Id).Value;
        var id = GetInternalId<T>(entry.Id);
        if (entity.ETag != entry.ETag)
        {
            return Errors.ETagMismatch(entity.ETag.ToString(), entry.ETag.ToString());
        }
        entry.ETag = Guid.NewGuid();
        cache.Set(id, entry);
        
        return entry;
    }

    public Task<T> GetAsync<T>(string id, CancellationToken ct = default) where T : class, IEntity
    {
        _lock.EnterReadLock();
        var result = Get<T>(id).Value;
        _lock.ExitReadLock();
        return Task.FromResult(result);
    }

    private ErrorOr<T> Get<T>(string id)
    {
        var entry = cache.Get<T>(GetInternalId<T>(id));
        if (entry is null)
        {
            return Error.NotFound();
        }
        
        return entry;
    }
    
    public async Task<List<T>> GetAllAsync<T>(CancellationToken ct = default)
    {
        return await GetAll<T>(ct);
    }
    
    private ValueTask<List<T>> GetAll<T>(CancellationToken ct = default)
    {
        var result = new List<T>();
        foreach (var key in _keys)
        {
            if (cache.TryGetValue(key, out var val))
            {
                result.Add((T)val);
            }
        }
        
        return new ValueTask<List<T>>(result);
    }

    private string GetInternalId<T>(string publicId)
    {
        return $"{typeof(T).Name}:{publicId}";
    }
}