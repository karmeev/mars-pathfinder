using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class WorldMapService : IWorldMapService
{
    public Task ChangeBotPositionAsync(Position position, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Position>> GetFuneralsAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Position CalculateDesiredPosition(Position currentPosition, IEnumerable<IOperatorCommand> commands)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryReachPosition(Position position, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}