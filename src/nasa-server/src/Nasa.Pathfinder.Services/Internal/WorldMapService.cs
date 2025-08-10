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
    public async Task<IPositionProject> ReachPosition(string mapId, Position currentPosition, 
        Stack<IOperatorCommand> commands, CancellationToken ct = default)
    {
        var start = new Position
        {
            X = currentPosition.X,
            Y = currentPosition.Y,
            Direction = Direction.N
        };
        var funerals = await funeralRepository.GetFuneralsAsync(mapId, ct);
        
        IPositionProject position = new PositionChanged(currentPosition);

        foreach (var command in commands)
        {
            Position current;
            switch (command)
            {
                case MoveRight or MoveLeft:
                    current = position.Position;
                    
                    current.Direction = Rotate(command, current.Direction);
                    position = new PositionChanged(current);
                    
                    Console.WriteLine($"Coordinates: {position.Position.X} x {position.Position.Y}, " +
                                      $"{position.Position.Direction}");
                    break;
                case MoveFront moveFrontCommand:
                    current = position.Position;
                    
                    var newPosition = await CalculateMoveFront(current, moveFrontCommand, 
                        funerals, mapId, ct);
                    
                    if (newPosition is PositionNotChanged)
                    {
                        Console.WriteLine($"Coordinates: {current.X} x {current.Y}, {current.Direction}");
                        continue;
                    }
                    
                    if (newPosition is PositionOutOfMap @out)
                    {
                        Console.WriteLine($"Coordinates: {newPosition.Position.X} x {newPosition.Position.Y}, " +
                                          $"{newPosition.Position.Direction}");
                        return @out;
                    }
            
                    position = newPosition;
                    Console.WriteLine($"Coordinates: {position.Position.X} x {position.Position.Y}, {position.Position.Direction}");
                    break;
                default:
                    continue;
            }
        }

        if (position.Position.X == start.X && position.Position.Y == start.Y && 
            position.Position.Direction == start.Direction)
        {
            return new PositionNotChanged(currentPosition);
        }
        
        return position;
    }

    private async Task<IPositionProject> CalculateMoveFront(Position position, MoveFront command, 
        IReadOnlyCollection<Funeral> funerals, string mapId, CancellationToken ct = default)
    {
        var previous = new Position
        {
            X = position.X,
            Y = position.Y,
            Direction = position.Direction
        };

        var isTrap = funerals.Any(x => x.Value.X == position.X 
                                       && x.Value.Y == position.Y 
                                       && x.Value.Direction == position.Direction);
        if (isTrap)
        {
            return PositionProject.NotChanged(position);
        }
        
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
        
        var isOutOfMap = false;
        var mapInfo = await mapRepository.TryGetAsync(mapId, ct);
        if (position.X > mapInfo.SizeX || position.Y > mapInfo.SizeY)
        {
            isOutOfMap = true;
        }

        if (isOutOfMap)
        {
            return PositionProject.OutOfMap(position, previous);
        }

        return PositionProject.Changed(position);
    }
    
    private static Direction Rotate(IOperatorCommand command, Direction direction)
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