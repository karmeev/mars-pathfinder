using ErrorOr;

namespace Nasa.Pathfinder.Infrastructure.Contracts.Exceptions;

public static class Errors
{
    public static Error ETagMismatch(string current, string incoming) =>
        Error.Conflict(
            code: "Concurrency.ETagMismatch",
            description: $"ETag mismatch: expected {current}, but got {incoming}");
}