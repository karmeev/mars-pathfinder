using Autofac;
using Nasa.Dashboard.View.Contracts;
using Nasa.Dashboard.View.Internal.Views.Engine;

namespace Nasa.Dashboard.View;

public static class ViewInitializer
{
    public static IViewNavigator Initialize(ILifetimeScope scope)
    {
        var navigator = new ViewNavigator(scope);
        return navigator;
    }
}