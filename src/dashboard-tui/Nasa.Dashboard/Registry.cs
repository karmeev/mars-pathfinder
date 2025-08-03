using Autofac;
using Nasa.Dashboard.Reducers.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard;

public static class Registry
{
    public static void RegisterDependencies(this ContainerBuilder builder)
    {
        Nasa.Dashboard.Registry.RegisterStore(builder);
        Nasa.Dashboard.View.Registry.RegisterViews(builder);
    }

    private static void RegisterStore(ContainerBuilder builder)
    {
        builder.Register(ctx =>
        {
            var store = new Nasa.Dashboard.Store.Store(new AppState());

            // // Register reducers
            store.RegisterReducer(BotsReducer.Reduce);
            //
            // // Register middleware
            // var botApi = ctx.Resolve<IBotApi>();
            store.RegisterMiddleware(new BotsMiddleware(store).HandleAsync);

            return store;
        }).As<IDispatcher>().As<IStore>().SingleInstance();
    }
}