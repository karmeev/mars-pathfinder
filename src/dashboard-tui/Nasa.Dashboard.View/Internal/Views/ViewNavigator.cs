using Autofac;
using Nasa.Dashboard.View.Contracts;
using Nasa.Dashboard.View.Internal.Core;

namespace Nasa.Dashboard.View.Internal.Views;

internal class ViewNavigator(ILifetimeScope scope) : IViewNavigator
{
    public void Run()
    {
        Console.Clear();
        
        IView? currentView = scope.Resolve<MainMenuView>();

        while (currentView != null)
        {
            currentView = currentView.Render();
        }

        Console.WriteLine("Program ended successfully...");
    }
}