using Autofac;
using Nasa.Dashboard.View.Contracts;
using Nasa.Dashboard.View.Internal;
using Nasa.Dashboard.View.Internal.Views;
using Nasa.Dashboard.View.Internal.Views.Engine;

namespace Nasa.Dashboard.View;

public static class Registry
{
    public static void RegisterViews(ContainerBuilder builder)
    {
        builder.RegisterModule<ViewModule>();
    }

    public static IViewNavigator InitializeViews(ILifetimeScope scope)
    {
        var navigator = new ViewNavigator(scope);
        return navigator;
    }
}