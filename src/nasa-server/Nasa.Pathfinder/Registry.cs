using Autofac;

namespace Nasa.Pathfinder;

public static class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddGrpc();
        Infrastructure.Registry.Register(services);
    }
    
    public static void RegisterContainer(ContainerBuilder builder)
    {
        Services.Registry.Register(builder);
        Data.Registry.Register(builder);
        Infrastructure.Registry.Register(builder);
    }
}