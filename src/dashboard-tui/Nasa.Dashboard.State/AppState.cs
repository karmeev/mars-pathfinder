using Nasa.Dashboard.Model;

namespace Nasa.Dashboard.State;

public record AppState
{
    public bool IsLoading { get; set; }
    public Bot? SelectedBot { get; set; }
    public IEnumerable<Bot> Bots { get; set; } = new List<Bot>();
}