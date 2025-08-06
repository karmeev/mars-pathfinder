using Autofac;
using Nasa.Pathfinder.Background;
using Nasa.Pathfinder.Hubs;
using Nasa.Pathfinder.Interceptors;
using Nasa.Pathfinder.Settings;

namespace Nasa.Pathfinder;

public static class Registry
{
    public static void RegisterServices(IServiceCollection services)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        services.AddOptions();
        
        services.AddHostedService<StartMessageHubBackgroundTask>();
        services.AddHostedService<MigrationBackgroundTask>();
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ExceptionInterceptor>();
            options.Interceptors.Add<ActivityInterceptor>();
        });
        Infrastructure.Registry.Register(services);
        services.Configure<EnableSettings>(configuration.GetSection("FeatureGate:Enablers"));
    }
    
    public static void RegisterContainer(ContainerBuilder builder)
    {
        Facades.Registry.Register(builder);
        Services.Registry.Register(builder);
        Data.Registry.Register(builder);
        Infrastructure.Registry.Register(builder);
        builder.RegisterType<MessageHub>().SingleInstance();
    }
}