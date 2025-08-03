using Nasa.Dashboard.State.States;

namespace Nasa.Dashboard.State;

public record AppState
{
    public BotState BotState { get; set; } = new();
    public ControlPanelState ControlPanelState { get; set; } = new();
}