namespace Nasa.Pathfinder.Domain.Core;

public interface IEntity
{
    public string Id { get; set; }
    public Guid ETag { get; set; }
}