using Nasa.Dashboard.State.Actions;

namespace Nasa.Dashboard.Store.Contracts;

public interface IDispatcher
{
    void Dispatch(IAction action);
}