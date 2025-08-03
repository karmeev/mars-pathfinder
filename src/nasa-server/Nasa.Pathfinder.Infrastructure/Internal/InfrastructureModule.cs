using Autofac;

namespace Nasa.Pathfinder.Infrastructure.Internal;

internal class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // builder.RegisterType<MyService>().As<IMyService>();
        // builder.RegisterType<MyRepository>().As<IRepository>();
    }
}