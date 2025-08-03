using Autofac;
using Nasa.Dashboard.Reducers.Internal.Bots;
using Nasa.Dashboard.Reducers.Internal.ControlPanel;
using Nasa.Dashboard.Reducers.Internal.System;
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
            store.RegisterReducer(SystemReducer.Reduce);
            store.RegisterReducer(BotsReducer.Reduce);
            store.RegisterReducer(ControlPanelReducer.Reduce);
            
            // Register middlewares
            var botService = ctx.Resolve<IBotsService>();
            var controlPanelService = ctx.Resolve<IControlPanelService>();
            var systemService = ctx.Resolve<ISystemService>();
            
            store.RegisterMiddleware(new BotsMiddleware(store, botService).HandleAsync);
            store.RegisterMiddleware(new ControlPanelMiddleware(store, controlPanelService).HandleAsync);
            store.RegisterMiddleware(new SystemMiddleware(store, systemService).HandleAsync);
            
            return store;
        }).As<IDispatcher>().As<IStore>().SingleInstance();
    }

    private static void RegisterServices(ContainerBuilder builder)
    {
        builder.RegisterType<BotsService>().As<IBotsService>();
        builder.RegisterType<ControlPanelService>().As<IControlPanelService>();
        builder.RegisterType<SystemService>().As<ISystemService>();
    }
}