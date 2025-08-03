using Nasa.Dashboard.Clients.Contracts;
using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal interface IControlPanelService
{
    Task SendMessageAsync(IMessage message, Bot bot);
    IAsyncEnumerable<IMessage> ReceiveMessagesAsync(CancellationToken ct = default);
    Task ExitFromPanel();
}

internal class ControlPanelService(IPathfinderClient client) : IControlPanelService
{
    public async Task SendMessageAsync(IMessage message, Bot bot)
    {
        await client.SendMessageAsync(bot.Id, message.Text);
    }

    public async Task ExitFromPanel()
    {
        await client.StopAsync();
    }

    public async IAsyncEnumerable<IMessage> ReceiveMessagesAsync(CancellationToken ct = default)
    {
        if (!client.IsAlreadyInChat())
        {
            client.StartChat();
        }
        
        await foreach (var message in client.GetIncomingMessagesAsync(ct))
        {
            yield return message;
        }
    }
}