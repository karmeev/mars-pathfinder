using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.State.States;

public record ControlPanelState
{
    public bool IsStreaming { get; init; }
    public Exception? StreamingError { get; init; }
    public IEnumerable<IMessage> Messages { get; init; } = [];
}