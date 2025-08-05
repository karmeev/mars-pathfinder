using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using NUnit.Framework;
using Pathfinder.Messages;
using Client = Pathfinder.Proto.PathfinderService.PathfinderServiceClient;

namespace Nasa.IntegrationTests.Pathfinder;

[TestFixture]
[Category("Integration")]
public class PathfinderServiceTests
{
    private GrpcChannel _channel;

    [SetUp]
    public void Setup()
    {
        _channel = GrpcChannel.ForAddress("http://localhost:5000");
    }

    [Test]
    public void Ping_ReturnsOk_HappyPath()
    {
        TestRunner<Client, PingResponse>
            .Arrange(() => new Client(_channel))
            .Act(sut => sut.Ping(new Empty()))
            .Assert(response => Assert.That(response.IsSuccessful, Is.True));
    }
    
    [Test]
    public void GetBots_ReturnsBotList_ShouldReturn3Bots()
    {
        TestRunner<Client, GetBotsResponse>
            .Arrange(() => new Client(_channel))
            .Act(sut => sut.GetBots(new Empty()))
            .Assert(response =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Bots, Is.Not.Empty);
                    Assert.That(response.Bots, Is.Not.Null);
                    Assert.That(response.Bots, Has.Count.EqualTo(3));
                });
            });
    }
    
    [Test]
    public void SelectBot_ReturnsBot_ShouldReturnBot()
    {
        TestRunner<Client, SelectBotResponse>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new Empty()).Bots.First();
                var result = sut.SelectBot(new SelectBotRequest
                {
                    BotId = bot.Id
                });
                return result;
            })
            .Assert(response =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Bot, Is.Not.Null);
                });
            });
    }
    
    [Test]
    public void SelectBot_BotAlreadyReleased_ShouldFail()
    {
        TestRunner<Client, RpcException>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new Empty()).Bots.First();
                try
                {
                    sut.SelectBot(new SelectBotRequest
                    {
                        BotId = bot.Id
                    });
                    
                    sut.SelectBot(new SelectBotRequest
                    {
                        BotId = bot.Id
                    });

                    return null;
                }
                catch (RpcException ex)
                {
                    return ex;
                }
            })
            .Assert(response =>
            {
                Assert.That(response, Is.Not.Null);
                Assert.That(response, Is.TypeOf<RpcException>());
                Assert.That(response.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
            });
    }
    
    [Test]
    public void ResetBot_ReturnsBot_ShouldInvalidArgument()
    {
        TestRunner<Client, ResetBotResponse>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new Empty()).Bots.First();
                bot = sut.SelectBot(new SelectBotRequest
                {
                    BotId = bot.Id
                }).Bot;

                var response = sut.ResetBot(new ResetBotRequest
                {
                    BotId = bot.Id
                });
                
                return response;
            })
            .Assert(response =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response.Bot, Is.Not.Null);
                });
            });
    }
    
    [Test]
    public void ResetBot_BotIsFree_ShouldInvalidArgument()
    {
        TestRunner<Client, RpcException>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new Empty()).Bots.First();
                bot = sut.SelectBot(new SelectBotRequest
                {
                    BotId = bot.Id
                }).Bot;

                try
                {
                    sut.ResetBot(new ResetBotRequest
                    {
                        BotId = bot.Id
                    });

                    sut.ResetBot(new ResetBotRequest
                    {
                        BotId = bot.Id
                    });
                    
                    return null;
                }
                catch (RpcException ex)
                {
                    return ex;
                }
            })
            .Assert(response =>
            {
                Assert.Multiple(() =>
                {
                    Assert.That(response, Is.Not.Null);
                    Assert.That(response, Is.TypeOf<RpcException>());
                    Assert.That(response.StatusCode, Is.EqualTo(StatusCode.InvalidArgument));
                });
            });
    }
    
    [TearDown]
    public void TearDown()
    {
        _channel.Dispose();
    }
}