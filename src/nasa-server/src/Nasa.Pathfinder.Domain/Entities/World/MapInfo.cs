using Nasa.Pathfinder.Domain.Core;

namespace Nasa.Pathfinder.Domain.Entities.World;

public class MapInfo : IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
    public int SizeX { get; set; }
    public int SizeY { get; set; }
}