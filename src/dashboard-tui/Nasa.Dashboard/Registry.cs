using Autofac;

namespace Nasa.Dashboard;

public static class Registry
{
    public static void RegisterDependencies(this ContainerBuilder builder)
    {
        Nasa.Dashboard.Reducers.Registry.RegisterStore(builder);
        Nasa.Dashboard.View.Registry.RegisterViews(builder);
    }
}