using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace Nasa.Pathfinder.Infrastructure;

public static class Registry
{
    public static void Register(IServiceCollection services)
    {
        services.AddMemoryCache();
    }
    
    public static void Register(ContainerBuilder builder)
    {
        //builder.RegisterModule<InfrastructureModule>();
    }
}