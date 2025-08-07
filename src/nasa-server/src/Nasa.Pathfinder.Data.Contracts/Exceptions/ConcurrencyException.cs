namespace Nasa.Pathfinder.Data.Contracts.Exceptions;

public class ConcurrencyException(string message, Exception? exception = null) : Exception(message, exception)
{
    public static void ThrowIfOptimistic(string message, Exception exception)
    {
        throw new ConcurrencyException(message, exception);
    }

    public static T ThrowIfOptimistic<T>(string message, Exception exception)
    {
        throw new ConcurrencyException(message, exception);
    }
}