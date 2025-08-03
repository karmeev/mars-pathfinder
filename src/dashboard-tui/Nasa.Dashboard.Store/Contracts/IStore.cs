using Nasa.Dashboard.State;

namespace Nasa.Dashboard.Store.Contracts;

public interface IStore
{
    public AppState CurrentState { get; }
    void Dispatch(IAction action);
}