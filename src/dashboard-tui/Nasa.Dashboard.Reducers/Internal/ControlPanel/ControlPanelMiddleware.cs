using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.ControlPanel;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal class ControlPanelMiddleware(IDispatcher dispatcher, IControlPanelService service)
{
    private bool _receivingStarted;
    private CancellationTokenSource? _cts;
    
    public async Task HandleAsync(IAction action)
    {
        switch (action)
        {
            case StreamingAction:
                if (_receivingStarted)
                    break;

                _receivingStarted = true;
                _cts = new CancellationTokenSource();

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await foreach (var message in service.ReceiveMessagesAsync(_cts.Token))
                        {
                            dispatcher.Dispatch(new BotMessageReceivedActionResult(message));
                        }
                    }
                    catch (Exception ex)
                    {
                        dispatcher.Dispatch(new StreamingErrorAction(ex));
                    }
                }, _cts.Token);

                dispatcher.Dispatch(new StreamingActionResult(_receivingStarted));
                break;

            case SendOperatorMessageAction send:

                try
                {
                    await service.SendMessageAsync(send.Message, send.Bot);
                    dispatcher.Dispatch(new SendOperatorMessageActionResult(send.Message));
                }
                catch (Exception ex)
                {
                    //smth happens, message don't send
                }
                break;
            
            case ExitControlPanelAction:
            {
                try
                {
                    await _cts?.CancelAsync()!;
                    await service.ExitFromPanel();
                    _receivingStarted = false;
                }
                catch (Exception ex)
                {
                    dispatcher.Dispatch(new StreamingErrorAction(ex));
                }

                break;
            }
        }
    }
}