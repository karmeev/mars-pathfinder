using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IWorldMapService
{
    Task<IPositionProject> ReachPosition(string mapId, Position currentPosition, 
        Stack<IOperatorCommand> commands, CancellationToken ct = default);
}