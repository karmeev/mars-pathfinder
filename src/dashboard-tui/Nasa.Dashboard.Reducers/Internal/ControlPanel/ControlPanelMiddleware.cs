using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.ControlPanel;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal class ControlPanelMiddleware(IDispatcher dispatcher, IControlPanelService service)
{
    public async Task HandleAsync(IAction action)
    {
        switch (action)
        {
            case StreamingAction:
                _ = StartStreamingAsync();
                break;

            case SendOperatorMessageAction send:
                await service.SendMessageAsync(send.Message);
                dispatcher.Dispatch(new SendOperatorMessageActionResult(send.Message));
                break;

            case BotMessageReceivedAction received:
                dispatcher.Dispatch(new BotMessageReceivedActionResult(received.Message));
                break;
        }
    }
    
    private async Task StartStreamingAsync()
    {
        try
        {
            await foreach (var message in service.ReceiveMessagesAsync())
            {
                dispatcher.Dispatch(new BotMessageReceivedAction(message));
            }

            dispatcher.Dispatch(new StreamingActionResult());
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new StreamingErrorAction(ex));
        }
    }
}