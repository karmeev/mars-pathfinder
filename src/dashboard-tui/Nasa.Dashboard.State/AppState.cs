using Nasa.Dashboard.State.States;

namespace Nasa.Dashboard.State;

public record AppState
{
    public bool IsConnected { get; init; }
    public int RetryConnectionCount { get; init; }
    public bool IsSystemExit { get; init; }
    public BotState BotState { get; set; } = new();
    public ControlPanelState ControlPanelState { get; set; } = new();
}