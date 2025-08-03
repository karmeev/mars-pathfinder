using Autofac;
using Nasa.Dashboard.View.Internal.Core;

namespace Nasa.Dashboard.View.Internal.Views;

internal class ViewFactory(ILifetimeScope scope) : IViewFactory
{
    public IView Create<T>() where T : IView
    {
        return scope.Resolve<T>();
    }
}