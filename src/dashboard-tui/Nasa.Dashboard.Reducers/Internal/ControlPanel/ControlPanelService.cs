using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal interface IControlPanelService
{
    Task SendMessageAsync(IMessage message);
    IAsyncEnumerable<IMessage> ReceiveMessagesAsync();
}

internal class ControlPanelService : IControlPanelService
{
    public Task SendMessageAsync(IMessage message)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<IMessage> ReceiveMessagesAsync()
    {
        throw new NotImplementedException();
    }
}