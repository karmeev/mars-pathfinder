using System.Diagnostics;
using Autofac;
using Nasa.Dashboard.View.Contracts;

namespace Nasa.Dashboard.Internal;

internal class NasaApp
{
    private IViewNavigator _navigator;

    public void InitializeViews(ILifetimeScope scope)
    {
        _navigator = View.Registry.InitializeViews(scope);
    }

    public void Run()
    {
        var activity = new Activity($"Operator - {Guid.NewGuid()}");
        activity.Start();
        Activity.Current = activity;
        
        if (_navigator == null)
        {
            throw new Exception("ViewNavigator not initialized");
        }
        
        _navigator.Run();
    }
}