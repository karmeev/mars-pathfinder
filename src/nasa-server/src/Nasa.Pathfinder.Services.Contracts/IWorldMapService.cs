using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IWorldMapService
{
    Task ChangeBotPositionAsync(Position position, CancellationToken ct = default);
    Task<List<Position>> GetFuneralsAsync(CancellationToken ct = default);
    Position CalculateDesiredPosition(Position currentPosition, Stack<IOperatorCommand> commands);
    Task<bool> TryReachPosition(Position position, CancellationToken ct = default);
}