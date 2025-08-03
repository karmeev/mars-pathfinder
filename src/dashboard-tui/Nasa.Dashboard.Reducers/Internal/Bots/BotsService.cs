using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.Reducers.Internal.Bots;

internal interface IBotsService
{
    Task<IEnumerable<Bot>> GetBotsAsync();
    Task<Bot> SelectBotAsync(string id);
    Task<bool> ResetBotAsync(Bot bot);
}

internal class BotsService : IBotsService
{
    public async Task<IEnumerable<Bot>> GetBotsAsync()
    {
        await Task.Delay(1000);
        return new List<Bot>
        {
            new Bot()
            {
                Id = "1",
                Name = "Bot1",
                Status = BotStatus.Available
            }
        };
    }

    public async Task<Bot> SelectBotAsync(string id)
    {
        await Task.CompletedTask;
        return new Bot()
        {
            Id = "1",
            Name = "Bot1",
            Status = BotStatus.Available
        };
    }

    public async Task<bool> ResetBotAsync(Bot bot)
    {
        await Task.CompletedTask;
        return true;
    }
}