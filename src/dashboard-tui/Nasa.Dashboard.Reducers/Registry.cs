using Autofac;
using Nasa.Dashboard.Reducers.Internal.Bots;
using Nasa.Dashboard.Reducers.Internal.ControlPanel;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers;

public static class Registry
{
    public static void RegisterStore(ContainerBuilder builder)
    {
        Nasa.Dashboard.Reducers.Registry.RegisterServices(builder);
        
        builder.RegisterType<AppState>().SingleInstance();
        builder.Register(ctx =>
        {
            var store = new Nasa.Dashboard.Store.Store(ctx.Resolve<AppState>());

            // // Register reducers
            store.RegisterReducer(BotsReducer.Reduce);
            store.RegisterReducer(ControlPanelReducer.Reduce);
            
            // Register middleware
            var botService = ctx.Resolve<IBotsService>();
            var controlPanelService = ctx.Resolve<IControlPanelService>();
            
            store.RegisterMiddleware(new BotsMiddleware(store, botService).HandleAsync);
            store.RegisterMiddleware(new ControlPanelMiddleware(store, controlPanelService).HandleAsync);
            
            return store;
        }).As<IDispatcher>().As<IStore>().SingleInstance();
    }

    private static void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<BotsService>().As<IBotsService>();
        builder.RegisterType<ControlPanelService>().As<IControlPanelService>();
    }
}