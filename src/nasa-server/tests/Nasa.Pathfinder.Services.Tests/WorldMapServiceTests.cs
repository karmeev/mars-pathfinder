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
    public void CalculateDesiredPosition_WhenWeGotListOfCommands_ShouldReturnsDestination()
    {
        TestRunner<WorldMapService, Position>
            .Arrange(() => new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object))
            .Act(sut =>
            {
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

                return sut.CalculateDesiredPosition(currentPosition, commands);
            })
            .Assert(position =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(position.X, Is.EqualTo(1));
                    Assert.That(position.Y, Is.EqualTo(1));
                    Assert.That(position.Direction, Is.EqualTo(Direction.E));
                });
            });
    }

    [Test]
    public void CalculateDesiredPosition_WhenDesiredPositionOutOfGrid_ShouldReturnsDestination()
    {
        TestRunner<WorldMapService, Position>
            .Arrange(() => new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object))
            .Act(sut =>
            {
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


                return sut.CalculateDesiredPosition(currentPosition, commands);
            })
            .Assert(position =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(position.X, Is.EqualTo(2));
                    Assert.That(position.Y, Is.EqualTo(4));
                    Assert.That(position.Direction, Is.EqualTo(Direction.S));
                });
            });
    }

    [Test]
    public async Task TryReachPosition_WhenFuneralWithDesiredPositionFindAndNotOutOfMap_ShouldReturnNotChanged()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";
                
        var desiredPosition = new Position
        {
            X = 1,
            Y = 2,
            Direction = Direction.W
        };

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 50,
                        SizeY = 50,
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
                            Value = desiredPosition
                        }
                    });

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
            .ThenAssertAsync(position =>
            {
                Assert.That(position, Is.TypeOf<PositionNotChanged>());
            });
    }
    
    [Test]
    public async Task TryReachPosition_WhenFuneralWithDesiredPositionOutOfMap_ShouldReturnNotChanged()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";
                
        var desiredPosition = new Position
        {
            X = 51,
            Y = 3,
            Direction = Direction.W
        };

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 50,
                        SizeY = 50,
                    });

                _mockFuneralRepository.Setup(x =>
                        x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
            .ThenAssertAsync(position =>
            {
                Assert.That(position, Is.TypeOf<PositionOutOfMap>());
            });
    }
    
    [Test]
    public async Task TryReachPosition_WhenBotCanMove_ShouldReturnChanged()
    {
        var mapId = $"map_{new Faker().Database.Random.Uuid()}";
                
        var desiredPosition = new Position
        {
            X = 4,
            Y = 3,
            Direction = Direction.W
        };

        await TestRunner<WorldMapService, IPositionProject>
            .Arrange(() =>
            {
                _mockMapRepository.Setup(x => x.TryGetAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new MapInfo
                    {
                        Id = mapId,
                        ETag = new Faker().Random.Guid(),
                        SizeX = 50,
                        SizeY = 50,
                    });

                _mockFuneralRepository.Setup(x =>
                        x.GetFuneralsAsync(mapId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Funeral> { FakeFuneral.Make(mapId) });

                return new WorldMapService(_mockMapRepository.Object, _mockFuneralRepository.Object);
            })
            .ActAsync(async sut => await sut.TryReachPosition(mapId, desiredPosition))
            .ThenAssertAsync(position =>
            {
                Assert.That(position, Is.TypeOf<PositionChanged>());
            });
    }
}