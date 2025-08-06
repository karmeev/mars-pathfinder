using System.Collections.Concurrent;
using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using Nasa.Pathfinder.Domain.Core;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;
using Nasa.Pathfinder.Infrastructure.Contracts.Exceptions;

namespace Nasa.Pathfinder.Infrastructure.Internal.Memory;

internal class MemoryDataContext(IMemoryCache cache) : IMemoryDataContext, IDisposable
{
    private readonly HashSet<string> _keys = [];
    private readonly ConcurrentDictionary<string, object> _entityLocks = new();

    public Task PushAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity
    {
        var lockObj = GetEntityLock<T>(entry.Id);
        lock (lockObj)
        {
            var id = GetInternalId<T>(entry.Id);
            cache.Set(id, entry);
            _keys.Add(id);
        }
        
        return Task.CompletedTask;
    }

    public Task<ErrorOr<T>> UpdateAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity
    {
        var lockObj = GetEntityLock<T>(entry.Id);
        lock (lockObj)
        {
            var result = Update(entry);
            return Task.FromResult(result);
        }
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
        var lockObj = GetEntityLock<T>(id);
        lock (lockObj)
        {
            var result = Get<T>(id).Value;
            return Task.FromResult(result);
        }
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
    
    private object GetEntityLock<T>(string id)
    {
        var key = GetInternalId<T>(id);
        return _entityLocks.GetOrAdd(key, _ => new object());
    }

    public void Dispose()
    {
        cache?.Dispose();
    }
}