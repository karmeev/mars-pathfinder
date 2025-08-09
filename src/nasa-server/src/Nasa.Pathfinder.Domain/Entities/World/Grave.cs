using Nasa.Pathfinder.Domain.Core;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Domain.Entities.World;

public class Grave : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public Position Value { get; init; }
    public string MapId { get; set; }
}