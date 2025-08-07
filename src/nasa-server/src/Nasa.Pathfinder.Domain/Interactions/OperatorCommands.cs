namespace Nasa.Pathfinder.Domain.Interactions;

public interface IOperatorCommand
{
    public int Steps { get; }
    public string Shorthand { get; }
}

public record UnknownCommand : IOperatorCommand
{
    public int Steps => 0;
    public string Shorthand => "";
}

public record MoveRight : IOperatorCommand
{
    public int Steps => 0;
    public string Shorthand => "R";
}

public record MoveLeft : IOperatorCommand
{
    public int Steps => 0;
    public string Shorthand => "L";
}

public record MoveFront : IOperatorCommand
{
    public int Steps => 1;
    public string Shorthand => "F";
}

public static class OperatorCommand
{
    public static string GetShorthand<T>() where T : IOperatorCommand
    {
        var instance = Activator.CreateInstance<T>();
        return instance.Shorthand;
    }
}