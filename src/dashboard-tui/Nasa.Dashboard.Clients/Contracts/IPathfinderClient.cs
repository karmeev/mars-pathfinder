using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.Clients.Contracts;

public interface IPathfinderClient
{
    Task<bool> PingAsync();
    Task DisconnectAsync();
    Task<IEnumerable<Bot>> GetBotsAsync();
    Task<Bot> SelectBotAsync(string botId);
    Task<bool> ResetBotAsync(string botId);
}