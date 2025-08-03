using Autofac;
using Nasa.Dashboard.Reducers.Internal.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers;

public static class Registry
{
    public static void RegisterStore(ContainerBuilder builder)
    {
        Nasa.Dashboard.Reducers.Registry.RegisterServices(builder);
        
        builder.Register(ctx =>
        {
            var store = new Nasa.Dashboard.Store.Store(new AppState());

            // // Register reducers
            store.RegisterReducer(BotsReducer.Reduce);
            
            // Register middleware
            var botService = ctx.Resolve<IBotsService>();
            store.RegisterMiddleware(new BotsMiddleware(store, botService).HandleAsync);

            return store;
        }).As<IDispatcher>().As<IStore>().SingleInstance();
    }

    private static void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<BotsService>().As<IBotsService>();
    }
}