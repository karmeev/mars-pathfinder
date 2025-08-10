using System.Numerics;
using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;

namespace Nasa.Pathfinder.Facades.Internal;

internal class BotFacade(
    IBotRepository repository,
    IMapRepository mapRepository) : IBotFacade
{
    public async Task<string> CreateWorldAsync(Vector2 coordinates, IEnumerable<Bot> bots, CancellationToken ct = default)
    {
        var map = new MapInfo
        {
            SizeX = (int)coordinates.X,
            SizeY = (int)coordinates.Y,
        };
        var createdMap = await mapRepository.AddAsync(map, ct);

        foreach (var bot in bots)
        {
            bot.MapId = createdMap.Id;
        }
        
        await repository.AddRangeAsync(bots.ToList(), ct);
        
        return createdMap.Id;
    }

    public async Task<IEnumerable<Bot>> GetBotsAsync(string mapId, CancellationToken ct = default)
    {
        try
        {
            var bots = await repository.GetBotsAsync(mapId, ct);
            return bots;
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
            return [];
        }
    }

    public async Task<Bot?> SelectBotAsync(string botId, CancellationToken ct = default)
    {
        try
        {
            var bot = await repository.ChangeBotStatusAsync(botId, BotStatus.Acquired, ct);
            return bot;
        }
        catch (DataException ex)
        {
            return BotAlreadyAcquiredException.ThrowIfInvalidData<Bot>(ex.Message, ex);
        }
        catch (ConcurrencyException ex)
        {
            return BotAlreadyAcquiredException.ThrowIfConcurrency<Bot>(ex);
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
            return new Bot();
        }
    }

    public async Task<Bot?> ResetBotAsync(string botId, CancellationToken ct = default)
    {
        try
        {
            var bot = await repository.ChangeBotStatusAsync(botId, BotStatus.Available, ct);
            return bot;
        }
        catch (DataException ex)
        {
            return BotAlreadyReleasedException.ThrowIfInvalidData<Bot>(ex.Message, ex);
        }
        catch (ConcurrencyException ex)
        {
            return BotAlreadyReleasedException.ThrowIfConcurrency<Bot>(ex);
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
            return new Bot();
        }
    }
}