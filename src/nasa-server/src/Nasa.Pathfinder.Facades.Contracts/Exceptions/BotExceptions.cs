namespace Nasa.Pathfinder.Facades.Contracts.Exceptions;

public class BotAlreadyReleasedException(string message, Exception exception) : Exception(message, exception)
{
    public static T Throw<T>(Exception exception) 
        => throw new BotAlreadyReleasedException(string.Empty, exception);
}

public class BotAlreadyAcquiredException(string message, Exception exception) : Exception(message, exception)
{
    public static T Throw<T>(Exception exception) 
        => throw new BotAlreadyAcquiredException(string.Empty, exception);
}