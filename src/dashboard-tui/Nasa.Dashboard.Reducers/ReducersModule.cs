using Autofac;
using Nasa.Dashboard.Reducers.Internal.Bots;
using Nasa.Dashboard.Reducers.Internal.ControlPanel;
using Nasa.Dashboard.Reducers.Internal.System;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers;

public class ReducersModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
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
        
        builder.RegisterType<BotsService>().As<IBotsService>();
        builder.RegisterType<ControlPanelService>().As<IControlPanelService>();
        builder.RegisterType<SystemService>().As<ISystemService>();
    }
}