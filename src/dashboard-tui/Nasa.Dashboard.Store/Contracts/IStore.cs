using Nasa.Dashboard.State;
using Nasa.Dashboard.State.Actions;

namespace Nasa.Dashboard.Store.Contracts;

public interface IStore : IDispatcher
{
    public AppState CurrentState { get; }
}