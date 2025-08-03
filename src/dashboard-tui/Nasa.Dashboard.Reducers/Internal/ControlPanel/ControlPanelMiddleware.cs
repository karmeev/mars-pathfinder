using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.ControlPanel;

internal class ControlPanelMiddleware(IDispatcher dispatcher, IControlPanelService service)
{
    public async Task HandleAsync(IAction action)
    {
        await Task.CompletedTask;
    }
}