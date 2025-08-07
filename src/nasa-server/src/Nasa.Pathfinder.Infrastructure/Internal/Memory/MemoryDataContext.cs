using System.Collections.Concurrent;
using ErrorOr;
using Microsoft.Extensions.Caching.Memory;
using Nasa.Pathfinder.Domain.Core;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;
using Nasa.Pathfinder.Infrastructure.Contracts.Exceptions;

namespace Nasa.Pathfinder.Infrastructure.Internal.Memory;

internal class MemoryDataContext(IMemoryCache cache) : IMemoryDataContext, IDisposable
{
    private readonly ConcurrentDictionary<string, object> _entityLocks = new();
    private readonly Dictionary<string, Type> _types = new();

    public void Dispose()
    {
        cache?.Dispose();
    }

    public Task PushAsync<T>(T entry, CancellationToken ct = default) where T : class, IEntity
    {
        var lockObj = GetEntityLock<T>(entry.Id);
        lock (lockObj)
        {
            var id = GetInternalId<T>(entry.Id);
            cache.Set(id, entry);
            _types.Add(id, entry.GetType());
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

    public Task<T?> GetAsync<T>(string id, CancellationToken ct = default) where T : class, IEntity
    {
        var lockObj = GetEntityLock<T>(id);
        lock (lockObj)
        {
            var result = Get<T>(id);
            if (result.IsError) return null;
            return Task.FromResult(result.Value);
        }
    }

    public async Task<List<T>> GetAllAsync<T>(CancellationToken ct = default)
    {
        return await GetAll<T>(ct);
    }

    public async Task<ErrorOr<object>> AcquireAsync<T>(string id, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        if (ct.IsCancellationRequested)
            return Error.Conflict();

        var @internal = GetInternalId<T>(id);
        _entityLocks.GetOrAdd(@internal, _ => new object());

        return Task.CompletedTask;
    }

    public async Task<ErrorOr<object>> ReleaseAsync<T>(string id, CancellationToken ct = default)
    {
        await Task.CompletedTask;

        if (ct.IsCancellationRequested)
            return Error.Conflict();

        var @internal = GetInternalId<T>(id);
        if (_entityLocks.TryRemove(@internal, out var lockObj)) return Task.CompletedTask;

        return Error.Failure();
    }

    private ErrorOr<T> Update<T>(T entry) where T : class, IEntity
    {
        var entity = Get<T>(entry.Id).Value;
        var id = GetInternalId<T>(entry.Id);
        if (entity.ETag != entry.ETag) return Errors.ETagMismatch(entity.ETag.ToString(), entry.ETag.ToString());
        entry.ETag = Guid.NewGuid();
        cache.Set(id, entry);

        return entry;
    }

    private ErrorOr<T> Get<T>(string id)
    {
        var entry = cache.Get<T>(GetInternalId<T>(id));
        if (entry is null) return Error.NotFound();

        return entry;
    }

    private ValueTask<List<T>> GetAll<T>(CancellationToken ct = default)
    {
        var result = new List<T>();
        var entityKeys = _types.Where(x => x.Value == typeof(T));
        foreach (var key in entityKeys)
        {
            if (cache.TryGetValue(key.Key, out var val))
                result.Add((T)val);
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

        if (_entityLocks.TryGetValue(key, out var lockObj)) return lockObj;

        return new { Id = Guid.NewGuid() };
    }
}