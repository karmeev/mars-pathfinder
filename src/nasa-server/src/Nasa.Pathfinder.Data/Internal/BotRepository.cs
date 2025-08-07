using ErrorOr;
using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Infrastructure.Contracts.DataContexts;

namespace Nasa.Pathfinder.Data.Internal;

internal class BotRepository(IDataContext dataContext) : IBotRepository
{
    public async Task<Bot?> TryGetAsync(string botId, CancellationToken ct = default)
    {
        return await dataContext.GetAsync<Bot>(botId, ct);
    }

    public async Task<IEnumerable<Bot>> GetBotsAsync(CancellationToken ct = default)
    {
        var bots = await dataContext.GetAllAsync<Bot>(ct);
        return bots;
    }

    public async Task<Bot?> ChangeBotStatusAsync(string botId, BotStatus status, CancellationToken ct = default)
    {
        await dataContext.AcquireAsync<Bot>(botId, ct);
        try
        {
            var bot = await TryGetAsync(botId, ct);
            
            if (bot is null)
                return DataException.ThrowIfIncorrectData<Bot>($"Bot is not exists");
            
            if (bot.Status == status)
                return DataException.ThrowIfIncorrectData<Bot>($"Bot already has status: {status}");

            bot.Status = status;

            ErrorOr<Bot> result = await dataContext.UpdateAsync(bot, ct);

            if (result.IsError)
            {
                var ex = new Exception($"{result.FirstError.Code}: {result.FirstError.Description}");
                return ConcurrencyException.ThrowIfOptimistic<Bot>("Bot already selected.", ex);
            }

            bot = result.Value;

            return bot;
        }
        finally
        {
            await dataContext.ReleaseAsync<Bot>(botId, ct);
        }
    }

    public async Task ChangeBotPositionAsync(string botId, Position position, CancellationToken ct = default)
    {
        await dataContext.AcquireAsync<Bot>(botId, ct);
        try
        {
            var bot = await TryGetAsync(botId, ct);
            if (bot is null)
            {
                DataException.ThrowIfIncorrectData("Bot is not exists");
                return;
            }
            
            bot.Position = position;
            var result = await dataContext.UpdateAsync(bot, ct);

            if (result.IsError)
            {
                var ex = new Exception($"{result.FirstError.Code}: {result.FirstError.Description}");
                ConcurrencyException.ThrowIfOptimistic("Bot already change position.", ex);
            }
        }
        finally
        {
            await dataContext.ReleaseAsync<Bot>(botId, ct);
        }
    }
}