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

            StreamingActionResult => state with
            {
                ControlPanelState = controlPanel with { IsStreaming = false }
            },

            StreamingErrorAction error => state with
            {
                ControlPanelState = controlPanel with
                {
                    IsStreaming = false,
                    StreamingError = error.Exception
                }
            },

            BotMessageReceivedAction => state,
            BotMessageReceivedActionResult msg => state with
            {
                ControlPanelState = controlPanel with
                {
                    Messages = controlPanel.Messages.Append(msg.Message)
                }
            },

            BotMessageErrorReceivedActionResult msg => state with
            {
                ControlPanelState = controlPanel with
                {
                    Messages = controlPanel.Messages.Append(msg.Message)
                }
            },
            
            SendOperatorMessageAction => state,
            SendOperatorMessageActionResult msg => state with
            {
                ControlPanelState = controlPanel with
                {
                    Messages = controlPanel.Messages.Append(msg.Message)
                }
            },
            _ => state
        };
    }
}