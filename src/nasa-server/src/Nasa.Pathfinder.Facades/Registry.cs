using Autofac;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Facades.Internal;

namespace Nasa.Pathfinder.Facades;

public static class Registry
{
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterType<BotFacade>().As<IBotFacade>();
        builder.RegisterType<MessageFacade>().As<IMessageFacade>();
    }
}