using Autofac;
using Nasa.Pathfinder.Consumers.Contracts;
using Nasa.Pathfinder.Consumers.Internal;
using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Consumers;

public static class Registry
{
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterType<BotWalkerConsumer>().As<IBotConsumer<WalkCommand>>();
        builder.RegisterType<BotDeadWalkerConsumer>().As<IBotConsumer<DeadCommand>>();
        builder.RegisterType<BotStandConsumer>().As<IBotConsumer<StandCommand>>();
        builder.RegisterType<BotInvalidCommandConsumer>().As<IBotConsumer<InvalidCommand>>();
        builder.RegisterType<BotConsumer>().As<IBotConsumer<MoveCommand>>();
    }
}