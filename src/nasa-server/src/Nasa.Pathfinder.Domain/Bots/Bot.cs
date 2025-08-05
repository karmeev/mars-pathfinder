using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Domain.Bots;

public class Bot
{
    public string Id { get; set; }
    public string Name { get; set; }
    public BotStatus Status { get; set; }
    public Position Position { get; set; }
    public string LastWords { get; set; }
}