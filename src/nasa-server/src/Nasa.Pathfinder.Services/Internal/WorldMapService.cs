using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class WorldMapService(
    IMapRepository mapRepository,
    IFuneralRepository funeralRepository) : IWorldMapService
{
    public Position CalculateDesiredPosition(Position currentPosition, Stack<IOperatorCommand> commands)
    {
        foreach (var command in commands) currentPosition = Move(currentPosition, command);

        return currentPosition;

        static Position Move(Position position, IOperatorCommand command)
        {
            if (command is MoveRight or MoveLeft) position.Direction = Rotate(command, position.Direction);

            switch (position.Direction)
            {
                case Direction.N:
                    position.Y += command.Steps;
                    break;
                case Direction.E:
                    position.X += command.Steps;
                    break;
                case Direction.S:
                    position.Y -= command.Steps;
                    break;
                case Direction.W:
                    position.X -= command.Steps;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return position;
        }

        static Direction Rotate(IOperatorCommand command, Direction direction)
        {
            return command switch
            {
                MoveLeft _ => direction switch
                {
                    Direction.N => Direction.W,
                    Direction.W => Direction.S,
                    Direction.S => Direction.E,
                    Direction.E => Direction.N,
                    _ => throw new ArgumentOutOfRangeException()
                },
                MoveRight _ => direction switch
                {
                    Direction.N => Direction.E,
                    Direction.E => Direction.S,
                    Direction.S => Direction.W,
                    Direction.W => Direction.N,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
        }
    }

    public async Task<IPositionProject> TryReachPosition(string mapId, Position position, 
        CancellationToken ct = default)
    {
        var funerals = await funeralRepository.GetFuneralsAsync(mapId, ct);
        var isTrap = funerals.Any(x => x.Value == position);
        
        var isOutOfMap = false;
        var mapInfo = await mapRepository.TryGetAsync(mapId, ct);

        if (position.X >= mapInfo.SizeX || position.Y >= mapInfo.SizeY)
        {
            isOutOfMap = true;
        }
        
        if (isTrap)
        {
            return PositionProject.NotChanged();
        }

        if (isOutOfMap)
        {
            return PositionProject.OutOfMap();
        }

        return PositionProject.Changed();
    }
}