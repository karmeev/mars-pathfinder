using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.System;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.System;

internal class SystemMiddleware(IDispatcher dispatcher, ISystemService service)
{
    public async Task HandleAsync(IAction action)
    {
        switch (action)
        {
            case PingAction:
                try
                {
                    var result = await service.PingAsync();
                    dispatcher.Dispatch(new PingActionResult(result));
                }
                catch (Exception ex)
                {
                    dispatcher.Dispatch(new PingActionResult(false));
                }
                break;
        }
    }
}