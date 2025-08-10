namespace Nasa.Pathfinder.Domain.World;

public interface IPositionProject
{
    public Position Position { get; }
}

public record PositionChanged(Position Position) : IPositionProject;
public record PositionNotChanged(Position Position) : IPositionProject;
public record PositionOutOfMap(Position Position, Position Previous) : IPositionProject;

public static class PositionProject
{
    public static IPositionProject Changed(Position position)
    {
        return new PositionChanged(position);
    }
    
    public static IPositionProject NotChanged(Position position)
    {
        return new PositionNotChanged(position);
    }
    
    public static IPositionProject OutOfMap(Position position, Position previous)
    {
        return new PositionOutOfMap(position, previous);
    }
}