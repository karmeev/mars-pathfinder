using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;

namespace Nasa.Pathfinder.Facades.Internal;

internal class BotFacade(IBotRepository repository) : IBotFacade
{
    public async Task<IEnumerable<Bot>> GetBotsAsync(CancellationToken ct = default)
    {
        try
        {
            var bots = await repository.GetBotsAsync(ct);
            return bots;
        }
        catch (Exception ex) when (ex is OperationCanceledException)
        {
            return [];
        }
    }

    public async Task<Bot> SelectBotAsync(string botId, CancellationToken ct = default)
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

    public async Task<Bot> ResetBotAsync(string botId, CancellationToken ct = default)
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