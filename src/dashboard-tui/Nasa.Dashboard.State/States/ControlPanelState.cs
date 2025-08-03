using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.State.States;

public record ControlPanelState
{
    public bool IsStreaming { get; init; }
    public bool IsWaiting { get; init; }
    public Exception? StreamingError { get; init; }
    public IReadOnlyList<IMessage> Messages { get; init; } = [];
}