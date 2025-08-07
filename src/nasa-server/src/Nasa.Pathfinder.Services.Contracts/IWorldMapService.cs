using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IWorldMapService
{
    Position CalculateDesiredPosition(Position currentPosition, Stack<IOperatorCommand> commands);
    Task<IPositionProject> TryReachPosition(string mapId, Position position, 
        CancellationToken ct = default);
}