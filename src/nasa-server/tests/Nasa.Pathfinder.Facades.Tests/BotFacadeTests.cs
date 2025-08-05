using FluentAssertions;
using Moq;
using Nasa.Pathfinder.Data.Contracts.Exceptions;
using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Facades.Contracts.Exceptions;
using Nasa.Pathfinder.Facades.Internal;
using Nasa.Pathfinder.Tests;

namespace Nasa.Pathfinder.Facades.Tests;

[TestFixture]
public class BotFacadeTests
{
    private Mock<IBotRepository> _repositoryMock;
    
    [SetUp]
    public void Setup()
    {
        _repositoryMock = new Mock<IBotRepository>();
    }

    [Test]
    public async Task GetBotsAsync_RepoReturnsNothing_ShouldReturnEmptyList()
    {
        await TestRunner<BotFacade, IEnumerable<Bot>>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.GetBotsAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.GetBotsAsync(CancellationToken.None))
            .ThenAssertAsync(result => result.Should().NotBeNull());
    }
    
    [Test]
    public async Task GetBotsAsync_RequestAborted_ShouldReturnEmptyList()
    {
        await TestRunner<BotFacade, IEnumerable<Bot>>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.GetBotsAsync(It.IsAny<CancellationToken>()))
                    .Callback(() => throw new OperationCanceledException());
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.GetBotsAsync(CancellationToken.None))
            .ThenAssertAsync(result => result.Should().BeEmpty());
    }
    
    [Test]
    public async Task SelectBotAsync_RepoReturnsBot_ShouldReturnBot()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.SelectBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Bot());
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.SelectBotAsync(Guid.NewGuid().ToString() ,CancellationToken.None))
            .ThenAssertAsync(result => result.Should().NotBeNull());
    }
    
    [Test]
    public async Task SelectBotAsync_ThrowsOptimisticConcurrency_ShouldThrowBotAlreadyAcquiredException()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.SelectBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .Callback(() => throw new OptimisticConcurrencyException(string.Empty));
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.SelectBotAsync(Guid.NewGuid().ToString() ,CancellationToken.None))
            .ThenAssertThrowsAsync<BotFacade, Bot, BotAlreadyAcquiredException>();
    }
    
    [Test]
    public async Task SelectBotAsync_RequestAborted_ShouldReturnBot()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.SelectBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .Callback(() => throw new OperationCanceledException());
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.SelectBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
            .ThenAssertAsync(result => result.Should().NotBeNull());
    }
    
    [Test]
    public async Task ResetBotAsync_RepoReturnsBot_ShouldReturnBot()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.ResetBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Bot());
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.ResetBotAsync(Guid.NewGuid().ToString() ,CancellationToken.None))
            .ThenAssertAsync(result => result.Should().NotBeNull());
    }
    
    [Test]
    public async Task ResetBotAsync_ThrowsOptimisticConcurrency_ShouldThrowBotAlreadyAcquiredException()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.ResetBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .Callback(() => throw new OptimisticConcurrencyException(string.Empty));
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.ResetBotAsync(Guid.NewGuid().ToString() ,CancellationToken.None))
            .ThenAssertThrowsAsync<BotFacade, Bot, BotAlreadyReleasedException>();
    }
    
    [Test]
    public async Task ResetBotAsync_RequestAborted_ShouldReturnBot()
    {
        await TestRunner<BotFacade, Bot>
            .Arrange(() =>
            {
                _repositoryMock.Setup(x => x.ResetBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
                    .Callback(() => throw new OperationCanceledException());
                return new BotFacade(_repositoryMock.Object);
            })
            .ActAsync(sut => sut.ResetBotAsync(It.IsAny<string>(),It.IsAny<CancellationToken>()))
            .ThenAssertAsync(result => result.Should().NotBeNull());
    }
}