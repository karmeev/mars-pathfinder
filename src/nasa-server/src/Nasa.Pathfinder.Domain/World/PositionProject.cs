namespace Nasa.Pathfinder.Domain.World;

public interface IPositionProject
{ }

public record PositionChanged : IPositionProject;
public record PositionNotChanged : IPositionProject;
public record PositionOutOfMap : IPositionProject;

public static class PositionProject
{
    public static IPositionProject Changed()
    {
        return new PositionChanged();
    }
    
    public static IPositionProject NotChanged()
    {
        return new PositionNotChanged();
    }
    
    public static IPositionProject OutOfMap()
    {
        return new PositionOutOfMap();
    }
}