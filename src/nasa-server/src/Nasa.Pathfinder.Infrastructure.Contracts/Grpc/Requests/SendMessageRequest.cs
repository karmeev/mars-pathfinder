namespace Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;

public record SendMessageRequest(
    string ClientId,
    string BotId,
    string Message,
    bool IsLost,
    bool IsInvalidCommand);