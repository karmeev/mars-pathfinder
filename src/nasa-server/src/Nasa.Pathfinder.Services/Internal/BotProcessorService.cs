using System.Collections.Concurrent;
using Autofac;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Services.Consumers.Interfaces;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Services.Internal;

internal class BotProcessorService(ILifetimeScope scope) : IBotProcessorService
{
    private readonly ConcurrentQueue<StandCommand> _queueStand = new();
    private readonly ConcurrentQueue<DeadCommand> _queueDead = new();
    private readonly ConcurrentQueue<WalkCommand> _queueWalk = new();
    private readonly ConcurrentQueue<MoveCommand> _queueMove = new();
    private readonly ConcurrentQueue<InvalidCommand> _queueInvalid = new();

    public void Publish(IBotCommand command)
    {
        switch (command)
        {
            case MoveCommand c:
                _queueMove.Enqueue(c);
                break;
            case WalkCommand c:
                _queueWalk.Enqueue(c);
                break;
            case StandCommand c:
                _queueStand.Enqueue(c);
                break;
            case DeadCommand c:
                _queueDead.Enqueue(c);
                break;
            case InvalidCommand c:
                _queueInvalid.Enqueue(c);
                break;
        }
    }

    public void RunConsumers()
    {
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueStand.TryDequeue(out var command))
                {
                    var consumer = scope.Resolve<IBotConsumer<StandCommand>>();
                    await consumer.Consume(command);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueDead.TryDequeue(out var command))
                {
                    var consumer = scope.Resolve<IBotConsumer<DeadCommand>>();
                    await consumer.Consume(command);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueWalk.TryDequeue(out var command))
                {
                    var consumer = scope.Resolve<IBotConsumer<WalkCommand>>();
                    await consumer.Consume(command);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueMove.TryDequeue(out var command))
                {
                    var consumer = scope.Resolve<IBotConsumer<MoveCommand>>();
                    await consumer.Consume(command);
                }

                await Task.Delay(5);
            });
        }
        
        for (int i = 0; i < 2; i++)
        {
            Task.Run(async () =>
            {
                if (_queueInvalid.TryDequeue(out var command))
                {
                    var consumer = scope.Resolve<IBotConsumer<InvalidCommand>>();
                    await consumer.Consume(command);
                }

                await Task.Delay(5);
            });
        }
    }
}