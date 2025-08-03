using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views.Components.Common;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class MainMenuView(IViewFactory factory, IStore store) : IView
{
    public IView? Render()
    {
        Header.RenderHeader(store.CurrentState);

        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[yellow]Select an option:[/]")
                .AddChoices("Acquire bot", "Start driving", "Exit"));

        return choice switch
        {
            "Acquire bot" => factory.Create<AcquireBotView>(),
            "Start driving" => factory.Create<DrivingView>(),
            "Exit" => null,
            _ => this
        };
    }
}
