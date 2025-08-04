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
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            return [];
        }
    }

    public async Task<Bot> SelectBotAsync(string botId, CancellationToken ct = default)
    {
        try
        {
            var bot = await repository.SelectBotAsync(botId, ct);
            return bot;
        }
        catch (OptimisticConcurrencyException ex)
        {
            return BotAlreadyAcquiredException.Throw<Bot>(ex);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            return new Bot();
        }
    }

    public async Task<Bot> ResetBotAsync(string botId, CancellationToken ct = default)
    {
        try
        {
            var bot = await repository.ResetBotAsync(botId, ct);
            return bot;
        }
        catch (OptimisticConcurrencyException ex)
        {
            return BotAlreadyReleasedException.Throw<Bot>(ex);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TimeoutException)
        {
            return new Bot();
        }
    }
}