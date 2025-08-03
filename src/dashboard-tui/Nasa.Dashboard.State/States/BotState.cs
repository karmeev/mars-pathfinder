using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.State.States;

public record BotState
{
    public bool IsLoading { get; init; }
    public Bot? SelectedBot { get; init; }
    public IEnumerable<Bot> Bots { get; init; } = [];
}