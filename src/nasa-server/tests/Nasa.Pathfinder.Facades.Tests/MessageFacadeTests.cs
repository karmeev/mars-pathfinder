using Bogus;
using Moq;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Entities.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Facades.Internal;
using Nasa.Pathfinder.Infrastructure.Contracts.Processors;
using Nasa.Pathfinder.Services.Contracts;
using Nasa.Pathfinder.Tests;

namespace Nasa.Pathfinder.Facades.Tests;

[TestFixture]
public class MessageFacadeTests
{
    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IBotRepository>();
        _decoderServiceMock = new Mock<IMessageDecoderService>();
        _worldMapServiceMock = new Mock<IWorldMapService>();
        _processorServiceMock = new Mock<IBotProcessor>();
    }

    private Mock<IBotRepository> _repositoryMock;
    private Mock<IMessageDecoderService> _decoderServiceMock;
    private Mock<IWorldMapService> _worldMapServiceMock;
    private Mock<IBotProcessor> _processorServiceMock;

    [Test]
    public async Task ReceiveMessageAsync_UnsupportedMessage_ShouldThrowsException()
    {
        await TestRunner<MessageFacade>
            .Arrange(() => new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object, _processorServiceMock.Object))
            .ActAsync(sut => sut.ReceiveMessageAsync(new BotMessage()))
            .ThenAssertThrowsAsync<MessageFacade, InvalidCastException>();
    }

    [Test]
    public async Task ReceiveMessageAsync_PositionReachable_ShouldMoveNext()
    {
        const string input = "FRFFL";
        var botId = new Faker().Random.Hash();
        var clientId = new Faker().Random.Hash();

        await TestRunner<MessageFacade>
            .Arrange(() =>
            {
                var commands = new Stack<IOperatorCommand>();
                commands.Push(new MoveFront());
                commands.Push(new MoveRight());
                commands.Push(new MoveFront());
                commands.Push(new MoveFront());
                commands.Push(new MoveLeft());

                _decoderServiceMock.Setup(x => x.DecodeOperatorMessage(
                        It.Is<string>(i => i.Equals(input))))
                    .Returns(commands);

                _repositoryMock.Setup(x => x.TryGetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Bot
                    {
                        Id = botId,
                        Name = new Faker().Hacker.Noun(),
                        Status = BotStatus.Acquired,
                        Position = new Position
                        {
                            X = 1,
                            Y = 1,
                            Direction = Direction.N
                        }
                    });

                return new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object, _processorServiceMock.Object);
            })
            .ActAsync(sut => sut.ReceiveMessageAsync(new OperatorMessage
            {
                BotId = botId,
                ClientId = clientId,
                Text = input
            }))
            .ThenAssertAsync(() =>
            {
                _processorServiceMock.Verify(x =>
                        x.Publish(It.IsAny<MoveCommand>()),
                    Times.Once);
            });
    }
}