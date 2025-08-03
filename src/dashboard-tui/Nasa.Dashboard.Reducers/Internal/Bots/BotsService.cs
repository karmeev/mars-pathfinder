using Nasa.Dashboard.Clients.Contracts;
using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.Reducers.Internal.Bots;

internal interface IBotsService
{
    Task<IEnumerable<Bot>> GetBotsAsync();
    Task<Bot> SelectBotAsync(string id);
    Task<bool> ResetBotAsync(Bot bot);
}

internal class BotsService(IPathfinderClient client) : IBotsService
{
    public async Task<IEnumerable<Bot>> GetBotsAsync()
    {
        var response = await client.GetBotsAsync();
        return response;
    }

    public async Task<Bot> SelectBotAsync(string id)
    {
        var response = await client.SelectBotAsync(id);
        return response;
    }

    public async Task<bool> ResetBotAsync(Bot bot)
    {
        var response = await client.ResetBotAsync(bot.Id);
        return response;
    }
}