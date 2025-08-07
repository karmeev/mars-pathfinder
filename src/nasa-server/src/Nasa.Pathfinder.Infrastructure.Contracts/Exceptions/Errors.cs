using ErrorOr;

namespace Nasa.Pathfinder.Infrastructure.Contracts.Exceptions;

public static class Errors
{
    public static Error ETagMismatch(string current, string incoming)
    {
        return Error.Conflict(
            "Concurrency.ETagMismatch",
            $"ETag mismatch: expected {current}, but got {incoming}");
    }
}