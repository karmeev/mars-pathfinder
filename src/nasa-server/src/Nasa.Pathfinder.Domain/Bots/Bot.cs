using Nasa.Pathfinder.Domain.Core;
using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Domain.Bots;

public class Bot : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public string Name { get; set; }
    public BotStatus Status { get; set; }
    public Position Position { get; set; }
    public string LastWords { get; set; }
}