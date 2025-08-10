using System.Diagnostics;
using System.Numerics;
using ErrorOr;
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
            .Act(sut => sut.GetBots(new GetBotsRequest
            {
                MapId = "1"
            }))
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
                var bot = sut.GetBots(new GetBotsRequest
                {
                    MapId = "1"
                }).Bots.First();
                var result = sut.SelectBot(new SelectBotRequest
                {
                    BotId = bot.Id
                });
                return result;
            })
            .Assert(response => { Assert.Multiple(() => { Assert.That(response.Bot, Is.Not.Null); }); });
    }

    [Test]
    public void SelectBot_BotAlreadyReleased_ShouldFail()
    {
        TestRunner<Client, RpcException>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new GetBotsRequest
                {
                    MapId = "1"
                }).Bots.First();
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
                var bot = sut.GetBots(new GetBotsRequest
                {
                    MapId = "1"
                }).Bots.First();
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
            .Assert(response => { Assert.Multiple(() => { Assert.That(response.Bot, Is.Not.Null); }); });
    }

    [Test]
    public void ResetBot_BotIsFree_ShouldInvalidArgument()
    {
        TestRunner<Client, RpcException>
            .Arrange(() => new Client(_channel))
            .Act(sut =>
            {
                var bot = sut.GetBots(new GetBotsRequest
                {
                    MapId = "1"
                }).Bots.First();
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

    [Test]
    //[Ignore("Right now, it's only for manual starting")]
    public async Task SendMessage_ApplyTestTaskBehavior_ShouldReturn3DifferentMessages()
    {
        const string message1 = "RFRFRFRF";
        const string message2 = "FRRFLLFFRRFLL";
        const string message3 = "LLFFFLFLFL";
        var botsMessage = new Dictionary<string, string>();
        var botsPositions = new Dictionary<string, Position>
        {
            { message1, new Position { X = 1, Y = 1, Direction = "E" } },
            { message2, new Position { X = 3, Y = 3, Direction = "N" } },
            { message3, new Position { X = 2, Y = 4, Direction = "S" } },
        };
        
        var mapId = string.Empty;

        string[] botNames = ["bot-12", "bot-14", "bot-15"];
        
        await TestRunner<Client, List<ErrorOr<SendMessageResponse>>>
            .ArrangeAsync(async () =>
            {
                var client = new Client(_channel);
                
                var createWorldResponse = await client.CreateWorldAsync(new CreateWorldRequest
                {
                    SizeX = 5,
                    SizeY = 3,
                    Bots = 
                    { 
                        new Bot() { Name = botNames[0], Position = new Position { X = 1, Y = 1, Direction = "E"} },
                        new Bot() { Name = botNames[1], Position = new Position { X = 3, Y = 2, Direction = "N"} },
                        new Bot() { Name = botNames[2], Position = new Position { X = 0, Y = 3, Direction = "W"} }
                    }
                });
                
                mapId = createWorldResponse.MapId;
                
                return client;
            })
            .ActAsync(async sut =>
            {
                var responses = new List<ErrorOr<SendMessageResponse>>();

                try
                {
                    //<--- First bot --->

                    var first = sut.GetBots(new GetBotsRequest
                    {
                        MapId = mapId
                    }).Bots.First(x => x.Name == botNames[0]);
                    var bot = sut.SelectBot(new SelectBotRequest
                    {
                        BotId = first.Id
                    }).Bot;
                    
                    await StartWorkflowAsync(bot, message1);

                    //<--- Second bot --->

                    var second = sut.GetBots(new GetBotsRequest
                    {
                        MapId = mapId
                    }).Bots.First(x => x.Name == botNames[1]);
                    bot = sut.SelectBot(new SelectBotRequest
                    {
                        BotId = second.Id
                    }).Bot;
                    
                    await StartWorkflowAsync(bot, message2);

                    //<--- Third bot --->

                    var third = sut.GetBots(new GetBotsRequest
                    {
                        MapId = mapId
                    }).Bots.First(x => x.Name == botNames[2]);
                    bot = sut.SelectBot(new SelectBotRequest
                    {
                        BotId = third.Id
                    }).Bot;
                    
                    await StartWorkflowAsync(bot, message3);
                }
                catch (Exception ex)
                {
                    responses.Add(Error.Failure(ex.Message));
                }

                return responses;


                async Task StartWorkflowAsync(Bot bot, string message)
                {
                    var stream = sut.SendMessage(new Metadata
                    {
                        { "TraceId", Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString() },
                        { "clientId", Guid.NewGuid().ToString() }
                    });

                    Thread.Sleep(1000);

                    botsMessage.Add(bot.Id, message);
                    await stream.RequestStream.WriteAsync(new SendMessageRequest
                    {
                        BotId = bot.Id,
                        Message = message
                    }, CancellationToken.None);

                    Thread.Sleep(2000);

                    var cts = new CancellationTokenSource();
                    var token = cts.Token;

                    try
                    {
                        while (await stream.ResponseStream.MoveNext(token))
                        {
                            var response = stream.ResponseStream.Current;
                            responses.Add(response);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        responses.Add(Error.Failure(ex.Message));
                    }

                    await stream.RequestStream.CompleteAsync();
                } 
            })
            .ThenAssertAsync(responses =>
            {
                var errors = new List<Error>();

                foreach (var response in responses)
                {
                    if (response.IsError)
                    {
                        errors.Add(response.FirstError);
                    }
                }

                try
                {
                    var client = new Client(_channel);
                    var bots = client.GetBots(new GetBotsRequest
                    {
                        MapId = mapId
                    }).Bots;

                    foreach (var bot in bots)
                    {
                        if (bot.Status == "Available")
                            errors.Add(Error.Unexpected("Bot is available",
                                $"Bot ({bot.Name}) is still available instead of be acquired"));

                        if (bot.Name == botNames[0])
                        {
                            CheckLocation(bot, new Vector2(1, 1), "E");
                        }
                        
                        if (bot.Name == botNames[1])
                        {
                            CheckLocation(bot, new Vector2(3, 3), "N");
                            
                            if (bot.Status != "Dead")
                                errors.Add(Error.Unexpected("Bot is available",
                                    $"Bot ({bot.Name}) is still available instead of be acquired"));
                        }
                        
                        if (bot.Name == botNames[2])
                        {
                            CheckLocation(bot, new Vector2(2, 3), "S");
                        }
                    }

                    void CheckLocation(Bot bot, Vector2 coordinates, string direction)
                    {
                        if (bot.Position.X != (int)coordinates.X 
                            && bot.Position.Y != (int)coordinates.Y 
                            && bot.Position.Direction != direction)
                        {
                            errors.Add(Error.Unexpected("Incorrect position",
                                $"Bot ({bot.Name}) has incorrect position"));
                        }
                    }
                }
                finally
                {
                    if (!errors.Any()) Assert.Pass();

                    if (errors.Count > 0)
                    {
                        foreach (var error in errors)
                        {
                            Console.WriteLine("{0}: {1}", error.Code, error.Description);
                        }

                        Assert.Fail();
                    }
                }
            });
    }

    [TearDown]
    public void TearDown()
    {
        _channel.Dispose();
    }
}