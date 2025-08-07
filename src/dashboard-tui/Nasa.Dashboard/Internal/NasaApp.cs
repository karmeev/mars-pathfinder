using Autofac;
using Nasa.Dashboard.View;
using Nasa.Dashboard.View.Contracts;

namespace Nasa.Dashboard.Internal;

internal class NasaApp
{
    private IViewNavigator _navigator;

    public void InitializeViews(ILifetimeScope scope)
    {
        _navigator = ViewInitializer.Initialize(scope);
    }

    public void Run()
    {
        if (_navigator == null)
        {
            throw new Exception("ViewNavigator not initialized");
        }
        
        _navigator.Run();
    }
}