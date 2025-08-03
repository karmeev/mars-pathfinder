using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class DrivingView(IViewFactory factory, IStore store) : IView
{
    private const string BackToMainMenu = "<-- Back to Main Menu";
    
    public IView Render()
    {
        var state = store.CurrentState;
        if (state.SelectedBot is null)
        {
            AnsiConsole.MarkupLine("Bot is [red]not[/] selected!");
            
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(BackToMainMenu));
            
            if (selected is BackToMainMenu)
                return factory.Create<MainMenuView>();
        }
        
        return this;
    }
}