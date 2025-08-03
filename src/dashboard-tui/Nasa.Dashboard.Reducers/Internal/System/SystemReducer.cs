using Nasa.Dashboard.State;
using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.System;

namespace Nasa.Dashboard.Reducers.Internal.System;

internal static class SystemReducer
{
    public static AppState Reduce(AppState state, IAction action)
    {
        return action switch
        {
            PingAction => state with
            {
                RetryConnectionCount = state.RetryConnectionCount + 1,
            },
            PingActionResult pingResult => state with
            {
                IsConnected = pingResult.IsSuccessful,
                IsSystemExit = state.RetryConnectionCount > 6
            },
            _ => state
        };
    }
}