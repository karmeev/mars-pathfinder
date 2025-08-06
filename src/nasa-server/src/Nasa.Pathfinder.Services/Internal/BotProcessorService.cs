using System.Collections.Concurrent;
using Autofac;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Consumers.Interfaces;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class BotProcessorService(
    IWorldMapService worldMap,
    ILifetimeScope scope) : IBotProcessorService
{
    private readonly ConcurrentQueue<StandCommand> _queueStand = new();
    private readonly ConcurrentQueue<DeadCommand> _queueDead = new();
    private readonly ConcurrentQueue<WalkCommand> _queueWalk = new();

    public async Task RouteAsync(IBotCommand command, CancellationToken ct = default)
    {
        switch (command)
        {
            case MoveCommand move:
                var isAccessible = await worldMap.TryReachPosition(move.DesiredPosition, ct);
                if (move.HasFunerals && isAccessible)
                {
                    var stand = new StandCommand(move.ClientId, move.Bot.Id, command.CorrelationId);
                    _queueStand.Enqueue(stand);
                }
                else
                {
                    if (!isAccessible)
                    {
                        var lost = new DeadCommand(move.ClientId, move.Bot.Id, move.DesiredPosition,
                            command.CorrelationId);
                        _queueDead.Enqueue(lost);
                        return;
                    }

                    var walk = new WalkCommand(move.ClientId, move.Bot.Id, move.DesiredPosition,
                        command.CorrelationId);
                    _queueWalk.Enqueue(walk);
                }
                break;
        }
    }

    public void RunConsumers()
    {
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueStand.TryDequeue(out var stand))
                {
                    var consumer = scope.Resolve<IBotConsumer<StandCommand>>();
                    await consumer.Consume(stand);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueDead.TryDequeue(out var dead))
                {
                    var consumer = scope.Resolve<IBotConsumer<DeadCommand>>();
                    await consumer.Consume(dead);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueWalk.TryDequeue(out var walk))
                {
                    var consumer = scope.Resolve<IBotConsumer<WalkCommand>>();
                    await consumer.Consume(walk);
                }

                await Task.Delay(5);
            });
        }
    }
}