using Nasa.Dashboard.State;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Store;

public interface IDispatcher
{
    void Dispatch(IAction action);
}

public class Store(AppState initialState) : IDispatcher, IStore
{
    private AppState _state = initialState;
    private readonly List<Func<AppState, IAction, AppState>> _reducers = new();
    private readonly List<Func<IAction, Task>> _middlewares = new();
    
    public event Action<AppState>? StateChanged;

    public AppState CurrentState => _state;
    
    public void RegisterReducer(Func<AppState, IAction, AppState> reducer)
        => _reducers.Add(reducer);
    
    public void RegisterMiddleware(Func<IAction, Task> middleware)
        => _middlewares.Add(middleware);

    public void Dispatch(IAction action)
    {
        foreach (var reducer in _reducers)
        {
            _state = reducer(_state, action);
        }

        StateChanged?.Invoke(_state);

        foreach (var middleware in _middlewares)
        {
            _ = middleware(action);
        }
    }
}