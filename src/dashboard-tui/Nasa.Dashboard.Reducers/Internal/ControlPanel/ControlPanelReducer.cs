using Nasa.Dashboard.State;
using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.ControlPanel;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal static class ControlPanelReducer
{
    public static AppState Reduce(AppState state, IAction action)
    {
        var controlPanel = state.ControlPanelState;

        return action switch
        {
            StreamingAction => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = true,
                    StreamingError = null
                }
            },
            StreamingActionResult streamingActionResult => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = streamingActionResult.IsStreaming,
                }
            },
            StreamingErrorAction streamingErrorAction => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = false,
                    StreamingError = streamingErrorAction.Exception
                }
            },
            
            BotMessageReceivedActionResult botMessageReceivedActionResult => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsWaiting = false,
                    Messages = controlPanel.Messages
                        .Append(botMessageReceivedActionResult.Message)
                        .ToList()
                }
            },
            BotMessageErrorReceivedActionResult errorReceivedActionResult => state with
            {
                ControlPanelState = controlPanel with
                {
                    Messages = controlPanel.Messages
                        .Append(errorReceivedActionResult.Message)
                        .ToList()
                }
            },
            
            SendOperatorMessageAction sendOperatorMessageAction => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsWaiting = true,
                    Messages = controlPanel.Messages
                        .Append(sendOperatorMessageAction.Message)
                        .ToList()
                }
            },
            SendOperatorMessageActionResult => state,
            
            ExitControlPanelAction => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = false
                }
            },
            
            CleanMessages => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = false,
                    Messages = []
                }
            },
            _ => state
        };
    }
}