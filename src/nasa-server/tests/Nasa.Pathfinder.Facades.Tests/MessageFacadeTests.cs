using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Internal;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Services.Contracts;
using Nasa.Pathfinder.Tests;

namespace Nasa.Pathfinder.Facades.Tests;

[TestFixture]
public class MessageFacadeTests
{
    private Mock<IBotRepository> _repositoryMock;
    private Mock<IMessageDecoderService> _decoderServiceMock;
    private Mock<IWorldMapService> _worldMapServiceMock;
    private Mock<IOperatorStream> _streamMock;
    
    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IBotRepository>();
        _decoderServiceMock = new Mock<IMessageDecoderService>();
        _worldMapServiceMock = new Mock<IWorldMapService>();
        _streamMock = new Mock<IOperatorStream>();
    }

    [Test]
    public async Task ReceiveMessageAsync_PositionReachable_ShouldMoveNext()
    {
        await TestRunner<MessageFacade>
            .Arrange(() =>
            {
                _decoderServiceMock.Setup(x => x.DecodeOperatorMessage(It.IsAny<string>()))
                    .Returns(new BotCommand(new List<IOperatorCommand>
                    {
                        new MoveFront(),
                        new MoveLeft(),
                        new MoveFront(),
                        new MoveFront(),
                        new MoveRight()
                    }));

                _repositoryMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Bot());

                _worldMapServiceMock.Setup(x => x.CalculateDesiredPosition(It.IsAny<Position>(),
                    It.IsAny<IEnumerable<IOperatorCommand>>()))
                    .Returns(new Position());

                //We don't have a funerals now
                _worldMapServiceMock.Setup(x => x.GetFuneralsAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);

                _worldMapServiceMock.Setup(x =>
                    x.TryReachPosition(It.IsAny<Position>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

                _decoderServiceMock.Setup(x =>
                        x.EncodeBotMessage(
                            It.IsAny<Position>(), 
                            It.Is<bool>(b => b == false), 
                            It.Is<bool>(b => b == false)))
                    .Returns("1 1 N");
                
                _streamMock.Setup(x => x.SendMessage(It.IsAny<SendMessageRequest>())).Verifiable();
                
                return new MessageFacade(_repositoryMock.Object, _decoderServiceMock.Object, 
                    _worldMapServiceMock.Object, _streamMock.Object);
            })
            .ActAsync(sut => sut.ReceiveMessageAsync(new OperatorMessage
            {
                BotId = "",
                ClientId = "",
                Text = "FLFFR"
            }))
            .ThenAssertDoesNotThrowAsync();
    }
}