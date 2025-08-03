using Nasa.Dashboard.Model.Messages;
using Bot = Nasa.Dashboard.Model.Bots.Bot;

namespace Nasa.Dashboard.Clients.Contracts;

public interface IPathfinderClient
{
    Task<bool> PingAsync();
    Task<IEnumerable<Bot>> GetBotsAsync();
    Task<Bot> SelectBotAsync(string botId);
    Task<bool> ResetBotAsync(string botId);
    void StartChat();
    Task StopAsync();
    bool IsAlreadyInChat();
    Task SendMessageAsync(string botId, string text);
    IAsyncEnumerable<IMessage> GetIncomingMessagesAsync(
        CancellationToken cancellationToken);
}