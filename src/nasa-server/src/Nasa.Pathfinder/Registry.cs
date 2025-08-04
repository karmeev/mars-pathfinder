using Autofac;
using Nasa.Pathfinder.Interceptors;
using Nasa.Pathfinder.Tmp;
using Pathfinder.Messages;

namespace Nasa.Pathfinder;

public static class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
            options.Interceptors.Add<ActivityInterceptor>();
        });
        Infrastructure.Registry.Register(services);
    }
    
    public static void RegisterContainer(ContainerBuilder builder)
    {
        Facades.Registry.Register(builder);
        Services.Registry.Register(builder);
        Data.Registry.Register(builder);
        Infrastructure.Registry.Register(builder);

        var str = new BotStorage();
        str.Bots = new()
        {
            new Bot { Id = "bot-123", Name = "TestBot", Status = "Available"},
            new Bot { Id = "bot-1233", Name = "TestBot2", Status = "Available"},
        };
        builder.RegisterInstance(str).As<BotStorage>().SingleInstance();
    }
}