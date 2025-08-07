using Moq;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Services.Internal;
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
}