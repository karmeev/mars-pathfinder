using Autofac;
using Nasa.Pathfinder.Services.Contracts;
using Nasa.Pathfinder.Services.Internal;

namespace Nasa.Pathfinder.Services;

public static class Registry
{
    public static void Register(ContainerBuilder builder)
    {
        builder.RegisterType<WorldMapService>().As<IWorldMapService>();
        builder.RegisterType<MessageDecoderService>().As<IMessageDecoderService>().SingleInstance();
        builder.RegisterType<BotProcessorService>().As<IBotProcessorService>().SingleInstance();
    }
}