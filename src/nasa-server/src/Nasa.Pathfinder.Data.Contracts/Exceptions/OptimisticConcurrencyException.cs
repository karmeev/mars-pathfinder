namespace Nasa.Pathfinder.Data.Contracts.Exceptions;

public class OptimisticConcurrencyException(string message) : Exception(message)
{
    public static void Throw(string message) =>
        throw new OptimisticConcurrencyException(message);
}