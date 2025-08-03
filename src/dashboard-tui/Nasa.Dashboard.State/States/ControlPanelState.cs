using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.State.States;

public record ControlPanelState
{
    public IEnumerable<IMessage> Messages { get; init; }
}