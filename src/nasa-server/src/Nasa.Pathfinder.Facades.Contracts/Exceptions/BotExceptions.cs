namespace Nasa.Pathfinder.Facades.Contracts.Exceptions;

public class BotAlreadyReleasedException(string message, Exception exception) : Exception(message, exception)
{
    public static T ThrowIfConcurrency<T>(Exception exception)
    {
        throw new BotAlreadyReleasedException(string.Empty, exception);
    }

    public static T ThrowIfInvalidData<T>(string message, Exception exception)
    {
        throw new BotAlreadyReleasedException($"Invalid data: {message}", exception);
    }
}

public class BotAlreadyAcquiredException(string message, Exception exception) : Exception(message, exception)
{
    public static T ThrowIfConcurrency<T>(Exception exception)
    {
        throw new BotAlreadyAcquiredException(string.Empty, exception);
    }

    public static T ThrowIfInvalidData<T>(string message, Exception exception)
    {
        throw new BotAlreadyAcquiredException($"Invalid data: {message}", exception);
    }
}