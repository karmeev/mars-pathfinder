using Bogus;
using Moq;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Services.Internal;
using Nasa.Pathfinder.Services.Tests.Fakes;
using Nasa.Pathfinder.Tests;
using NUnit.Framework;

namespace Nasa.Pathfinder.Services.Tests;

[TestFixture]
public class WorldMapServiceTests
{
    [SetUp]
    public void Setup()
    {
        _mockMapRepository = new Mock<IMapRepository>();
        _mockFuneralRepository = new Mock<IFuneralRepository>();
    }

    private Mock<IMapRepository> _mockMapRepository;
    private Mock<IFuneralRepository> _mockFuneralRepository;


    [Test]
    public async Task ReachPosition_WhenFuneralWithDesiredPositionFindAndNotOutOfMap_ShouldReturnNotChanged()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";

        var currentPosition = new Position
        {
            X = 0,
            Y = 3,
            Direction = Direction.W
        };
        
        var commands = new Stack<IOperatorCommand>();
        commands.Push(new MoveLeft());

        commands.Push(new MoveFront());
        commands.Push(new MoveLeft());

        commands.Push(new MoveFront());
        commands.Push(new MoveLeft());

        commands.Push(new MoveFront());
        commands.Push(new MoveFront());
        commands.Push(new MoveFront());

        commands.Push(new MoveLeft());
        commands.Push(new MoveLeft());

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 5,
                        SizeY = 3,
                    });

                _mockFuneralRepository.Setup(x =>
                        x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Funeral>
                    {
                        new()
                        {
                            Id = new Faker().Random.Guid().ToString(),
                            ETag = new Faker().Random.Guid(),
                            MapId = mapId,
                            Value = new Position
                            {
                                X = 3,
                                Y = 3,
                                Direction = Direction.N
                            }
                        }
                    });

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.ReachPosition(mapId, currentPosition, commands))
            .ThenAssertAsync(positionProject =>
            {
                var desiredPosition = new Position
                {
                    X = 2,
                    Y = 3,
                    Direction = Direction.S
                };
                
                Assert.Multiple(() =>
                {
                    Assert.That(positionProject, Is.TypeOf<PositionChanged>());
                    Assert.That(positionProject.Position.X, Is.EqualTo(desiredPosition.X));
                    Assert.That(positionProject.Position.Y, Is.EqualTo(desiredPosition.Y));
                    Assert.That(positionProject.Position.Direction, Is.EqualTo(desiredPosition.Direction));
                });
                
            });
    }
    
    [Test]
    public async Task ReachPosition_WhenBotCanMove_ShouldReturnChanged()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";

        var currentPosition = new Position
        {
            X = 1,
            Y = 1,
            Direction = Direction.E
        };
        
        var commands = new Stack<IOperatorCommand>();
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 5,
                        SizeY = 3,
                    });

                _mockFuneralRepository.Setup(x =>
                        x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Funeral>());

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.ReachPosition(mapId, currentPosition, commands))
            .ThenAssertAsync(positionProject =>
            {
                var desiredPosition = new Position
                {
                    X = currentPosition.X,
                    Y = currentPosition.Y,
                    Direction = currentPosition.Direction
                };
                
                Assert.Multiple(() =>
                {
                    Assert.That(positionProject, Is.TypeOf<PositionChanged>());
                    Assert.That(positionProject.Position.X, Is.EqualTo(desiredPosition.X));
                    Assert.That(positionProject.Position.Y, Is.EqualTo(desiredPosition.Y));
                    Assert.That(positionProject.Position.Direction, Is.EqualTo(desiredPosition.Direction));
                });
                
            });
    }
    
    [Test]
    public async Task ReachPosition_WhenDesiredPositionOutOfGrid_ShouldReturnsDestination()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";

        var currentPosition = new Position
        {
            X = 3,
            Y = 2,
            Direction = Direction.N
        };
        
        var commands = new Stack<IOperatorCommand>();
        commands.Push(new MoveLeft());
        commands.Push(new MoveLeft());
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());
        commands.Push(new MoveRight());
        commands.Push(new MoveFront());
        commands.Push(new MoveFront());
        commands.Push(new MoveLeft());
        commands.Push(new MoveLeft());
        commands.Push(new MoveFront());
        commands.Push(new MoveRight());
        commands.Push(new MoveRight());
        commands.Push(new MoveFront());

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 5,
                        SizeY = 3,
                    });

                _mockFuneralRepository.Setup(x =>
                        x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Funeral>());

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.ReachPosition(mapId, currentPosition, commands))
            .ThenAssertAsync(positionProject =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(positionProject, Is.TypeOf<PositionOutOfMap>());
                    
                    Assert.That(positionProject.Position.X, Is.EqualTo(3));
                    Assert.That(positionProject.Position.Y, Is.EqualTo(4));
                    Assert.That(positionProject.Position.Direction, Is.EqualTo(Direction.N));
                    
                    var @out = positionProject as PositionOutOfMap;
                    Assert.That(@out.Previous.X, Is.EqualTo(3));
                    Assert.That(@out.Previous.Y, Is.EqualTo(3));
                    Assert.That(@out.Previous.Direction, Is.EqualTo(Direction.N));
                });
                
            });
    }
    
    
    //
    // [Test]
    // public async Task ReachPosition_WhenFuneralWithDesiredPositionOutOfMap_ShouldReturnNotChanged()
    // {
    //     var mapId = $"map_{new Faker().Database.Random.Uuid()}";
    //
    //     var desiredPosition = new Position
    //     {
    //         X = 51,
    //         Y = 3,
    //         Direction = Direction.W
    //     };
    //
    //     await TestRunner<WorldMapService, IPositionProject>
    //         .Arrange(() =>
    //         {
    //             _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new MapInfo
    //                 {
    //                     Id = mapId,
    //                     ETag = new Faker().Random.Guid(),
    //                     SizeX = 50,
    //                     SizeY = 50,
    //                 });
    //
    //             _mockFuneralRepository.Setup(x =>
    //                     x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });
    //
    //             return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
    //         })
    //         .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
    //         .ThenAssertAsync(position => { Assert.That(position, Is.TypeOf<PositionOutOfMap>()); });
    // }
    //
    // [Test]
    // public async Task ReachPosition_WhenBotCanMove_ShouldReturnChanged()
    // {
    //     var mapId = $"map_{new Faker().Database.Random.Uuid()}";
    //
    //     var desiredPosition = new Position
    //     {
    //         X = 4,
    //         Y = 3,
    //         Direction = Direction.W
    //     };
    //
    //     await TestRunner<WorldMapService, IPositionProject>
    //         .Arrange(() =>
    //         {
    //             _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new MapInfo
    //                 {
    //                     Id = mapId,
    //                     ETag = new Faker().Random.Guid(),
    //                     SizeX = 50,
    //                     SizeY = 50,
    //                 });
    //
    //             _mockFuneralRepository.Setup(x =>
    //                     x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });
    //
    //             return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
    //         })
    //         .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
    //         .ThenAssertAsync(position => { Assert.That(position, Is.TypeOf<PositionChanged>()); });
    // }
    //
    //
    // [Test]
    // public void CalculateDesiredPosition_WhenWeGotListOfCommands_ShouldReturnsDestination()
    // {
    //     TestRunner<WorldMapService, Position>
    //         .Arrange(() => new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object))
    //         .Act(sut =>
    //         {
    //             var currentPosition = new Position
    //             {
    //                 X = 1,
    //                 Y = 1,
    //                 Direction = Direction.E
    //             };
    //
    //             var commands = new Stack<IOperatorCommand>();
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveRight());
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveRight());
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveRight());
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveRight());
    //
    //             return sut.CalculateDesiredPosition(currentPosition, commands);
    //         })
    //         .Assert(position =>
    //         {
    //             Assert.Multiple(() =>
    //             {
    //                 Assert.That(position.X, Is.EqualTo(1));
    //                 Assert.That(position.Y, Is.EqualTo(1));
    //                 Assert.That(position.Direction, Is.EqualTo(Direction.E));
    //             });
    //         });
    // }
    //
    // [Test]
    // public void CalculateDesiredPosition_WhenDesiredPositionOutOfGrid_ShouldReturnsDestination()
    // {
    //     TestRunner<WorldMapService, Position>
    //         .Arrange(() => new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object))
    //         .Act(sut =>
    //         {
    //             var currentPosition = new Position
    //             {
    //                 X = 0,
    //                 Y = 3,
    //                 Direction = Direction.W
    //             };
    //
    //             _mockFuneralRepository.Setup(x => x.GetFuneralsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync([
    //                     new Funeral
    //                     {
    //                         Id = "1",
    //                         ETag = Guid.NewGuid(),
    //                         MapId = "s",
    //                         Value = new Position
    //                         {
    //                             X = 2,
    //                             Y = 4,
    //                             Direction = Direction.E
    //                         }
    //                     }
    //                 ]);
    //
    //             var commands = new Stack<IOperatorCommand>();
    //             commands.Push(new MoveLeft());
    //
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveLeft());
    //
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveLeft());
    //
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveFront());
    //             commands.Push(new MoveFront());
    //
    //             commands.Push(new MoveLeft());
    //             commands.Push(new MoveLeft());
    //
    //
    //             return sut.CalculateDesiredPosition(currentPosition, commands);
    //         })
    //         .Assert(position =>
    //         {
    //             Assert.Multiple(() =>
    //             {
    //                 Assert.That(position.X, Is.EqualTo(2));
    //                 Assert.That(position.Y, Is.EqualTo(4));
    //                 Assert.That(position.Direction, Is.EqualTo(Direction.S));
    //             });
    //         });
    // }
    //
    // [Test]
    // public async Task TryReachPosition_WhenFuneralWithDesiredPositionFindAndNotOutOfMap_ShouldReturnNotChanged()
    // {
    //     var mapId = $"map_{new Faker().Database.Random.Uuid()}";
    //
    //     var desiredPosition = new Position
    //     {
    //         X = 2,
    //         Y = 4,
    //         Direction = Direction.S
    //     };
    //
    //     await TestRunner<WorldMapService, IPositionProject>
    //         .Arrange(() =>
    //         {
    //             _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new MapInfo
    //                 {
    //                     Id = mapId,
    //                     ETag = new Faker().Random.Guid(),
    //                     SizeX = 5,
    //                     SizeY = 3,
    //                 });
    //
    //             _mockFuneralRepository.Setup(x =>
    //                     x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new List<Funeral>
    //                 {
    //                     new()
    //                     {
    //                         Id = new Faker().Random.Guid().ToString(),
    //                         ETag = new Faker().Random.Guid(),
    //                         MapId = mapId,
    //                         Value = desiredPosition
    //                     }
    //                 });
    //
    //             return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
    //         })
    //         .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
    //         .ThenAssertAsync(position => { Assert.That(position, Is.TypeOf<PositionNotChanged>()); });
    // }
    //
    // [Test]
    // public async Task TryReachPosition_WhenFuneralWithDesiredPositionOutOfMap_ShouldReturnNotChanged()
    // {
    //     var mapId = $"map_{new Faker().Database.Random.Uuid()}";
    //
    //     var desiredPosition = new Position
    //     {
    //         X = 51,
    //         Y = 3,
    //         Direction = Direction.W
    //     };
    //
    //     await TestRunner<WorldMapService, IPositionProject>
    //         .Arrange(() =>
    //         {
    //             _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new MapInfo
    //                 {
    //                     Id = mapId,
    //                     ETag = new Faker().Random.Guid(),
    //                     SizeX = 50,
    //                     SizeY = 50,
    //                 });
    //
    //             _mockFuneralRepository.Setup(x =>
    //                     x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });
    //
    //             return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
    //         })
    //         .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
    //         .ThenAssertAsync(position => { Assert.That(position, Is.TypeOf<PositionOutOfMap>()); });
    // }
    //
    // [Test]
    // public async Task TryReachPosition_WhenBotCanMove_ShouldReturnChanged()
    // {
    //     var mapId = $"map_{new Faker().Database.Random.Uuid()}";
    //
    //     var desiredPosition = new Position
    //     {
    //         X = 4,
    //         Y = 3,
    //         Direction = Direction.W
    //     };
    //
    //     await TestRunner<WorldMapService, IPositionProject>
    //         .Arrange(() =>
    //         {
    //             _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new MapInfo
    //                 {
    //                     Id = mapId,
    //                     ETag = new Faker().Random.Guid(),
    //                     SizeX = 50,
    //                     SizeY = 50,
    //                 });
    //
    //             _mockFuneralRepository.Setup(x =>
    //                     x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });
    //
    //             return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
    //         })
    //         .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
    //         .ThenAssertAsync(position => { Assert.That(position, Is.TypeOf<PositionChanged>()); });
    // }
}