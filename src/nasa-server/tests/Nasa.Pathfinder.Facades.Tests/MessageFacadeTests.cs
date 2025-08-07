using Bogus;
using Moq;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.Messages;
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
            .Arrange(() => new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object,
                _worldMapServiceMock.Object, _processorServiceMock.Object))
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

                _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
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

                _worldMapServiceMock.Setup(x => x.CalculateDesiredPosition(It.IsAny<Position>(),
                        It.IsAny<Stack<IOperatorCommand>>()))
                    .Returns(new Position
                    {
                        X = 4,
                        Y = 3,
                        Direction = Direction.N
                    });

                return new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object,
                    _worldMapServiceMock.Object, _processorServiceMock.Object);
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

    // [Test]
    // public async Task ReceiveMessageAsync_HasFuneral_ShouldStayAtPlace()
    // {
    //     const string input = "FRFFL";
    //     var botId = new Faker().Random.Hash();
    //     var clientId = new Faker().Random.Hash();
    //     
    //     var desiredPosition = new Position
    //     {
    //         X = 4,
    //         Y = 3,
    //         Direction = Direction.N
    //     };
    //     
    //     await TestRunner<MessageFacade>
    //         .Arrange(() =>
    //         {
    //             _decoderServiceMock.Setup(x => x.DecodeOperatorMessage(
    //                     It.Is<string>(i => i.Equals(input))))
    //                 .Returns(new List<IOperatorCommand>
    //                 {
    //                     new MoveFront(),
    //                     new MoveRight(),
    //                     new MoveFront(),
    //                     new MoveFront(),
    //                     new MoveLeft()
    //                 });
    //
    //             _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new Bot
    //                 {
    //                     Id = botId,
    //                     Name = new Faker().Hacker.Noun(),
    //                     Status = BotStatus.Acquired,
    //                     Position = new Position
    //                     {
    //                         X = 1,
    //                         Y = 1,
    //                         Direction = Direction.N
    //                     }
    //                 });
    //
    //             _worldMapServiceMock.Setup(x => x.CalculateDesiredPosition(It.IsAny<Position>(),
    //                 It.IsAny<IEnumerable<IOperatorCommand>>()))
    //                 .Returns(desiredPosition);
    //
    //             _worldMapServiceMock.Setup(x => x.GetFuneralsAsync(It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync([desiredPosition]);
    //
    //             _worldMapServiceMock.Setup(x =>
    //                 x.TryReachPosition(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(true);
    //
    //             _decoderServiceMock.Setup(x =>
    //                     x.EncodeBotMessage(
    //                         It.IsAny<Position>(), 
    //                         It.Is<bool>(b => b == false), 
    //                         It.Is<bool>(b => b == false)))
    //                 .Returns("4 3 N");
    //             
    //             return new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object, 
    //                 _worldMapServiceMock.Object);
    //         })
    //         .ActAsync(sut => sut.ReceiveMessageAsync(new OperatorMessage
    //         {
    //             BotId = botId,
    //             ClientId = clientId,
    //             Text = input
    //         }))
    //         .ThenAssertAsync(() =>
    //         {
    //             _worldMapServiceMock.Verify(x =>
    //                     x.ChangeBotPositionAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()),
    //                 Times.Never);
    //             
    //             _streamMock.Verify(x => x.SendMessage(It.IsAny<SendMessageRequest>()), Times.Once);
    //         });
    // }
    //
    // [Test]
    // public async Task ReceiveMessageAsync_CantReachPosition_ShouldLost()
    // {
    //     const string input = "FRFFL";
    //     var botId = new Faker().Random.Hash();
    //     var clientId = new Faker().Random.Hash();
    //     
    //     var desiredPosition = new Position
    //     {
    //         X = 199,
    //         Y = 3,
    //         Direction = Direction.N
    //     };
    //     
    //     await TestRunner<MessageFacade>
    //         .Arrange(() =>
    //         {
    //             _decoderServiceMock.Setup(x => x.DecodeOperatorMessage(
    //                     It.Is<string>(i => i.Equals(input))))
    //                 .Returns([]);
    //
    //             _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(new Bot
    //                 {
    //                     Id = botId,
    //                     Name = new Faker().Hacker.Noun(),
    //                     Status = BotStatus.Acquired,
    //                     Position = new Position
    //                     {
    //                         X = 1,
    //                         Y = 1,
    //                         Direction = Direction.N
    //                     }
    //                 });
    //
    //             _worldMapServiceMock.Setup(x => x.CalculateDesiredPosition(It.IsAny<Position>(),
    //                 It.IsAny<IEnumerable<IOperatorCommand>>()))
    //                 .Returns(desiredPosition);
    //
    //             _worldMapServiceMock.Setup(x => x.GetFuneralsAsync(It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync([]);
    //
    //             _worldMapServiceMock.Setup(x =>
    //                 x.TryReachPosition(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
    //                 .ReturnsAsync(false);
    //
    //             _decoderServiceMock.Setup(x =>
    //                     x.EncodeBotMessage(
    //                         It.IsAny<Position>(), 
    //                         It.Is<bool>(b => b == false), 
    //                         It.Is<bool>(b => b == false)))
    //                 .Returns("199 3 N");
    //             
    //             return new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object, 
    //                 _worldMapServiceMock.Object);
    //         })
    //         .ActAsync(sut => sut.ReceiveMessageAsync(new OperatorMessage
    //         {
    //             BotId = botId,
    //             ClientId = clientId,
    //             Text = input
    //         }))
    //         .ThenAssertAsync(() =>
    //         {
    //             _worldMapServiceMock.Verify(x =>
    //                     x.ChangeBotPositionAsync(It.IsAny<Position>(), It.IsAny<CancellationToken>()),
    //                 Times.Once);
    //             
    //             _streamMock.Verify(x => 
    //                 x.SendMessage(It.Is<SendMessageRequest>(r => r.IsLost)), Times.Once);
    //         });
    // }
}