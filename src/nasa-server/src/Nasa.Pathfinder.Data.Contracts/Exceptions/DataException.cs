namespace Nasa.Pathfinder.Data.Contracts.Exceptions;

public class DataException(string message) : Exception(message)
{
    public static void ThrowIfIncorrectData(string message)
    {
        throw new DataException($"Data error: {message}");
    }
    
    public static T ThrowIfIncorrectData<T>(string message)
    {
        throw new DataException($"Data error: {message}");
    }
}