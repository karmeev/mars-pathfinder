using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views.Components.Common;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class DrivingView(IViewFactory factory, IStore store) : IView
{
    private const string BackToMainMenu = "<-- Back to Main Menu";
    
    public IView Render()
    {
        var state = store.CurrentState;
        
        Header.RenderHeader(state);
        
        if (state.SelectedBot is null)
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Bot is [red]not[/] selected!")
                    .AddChoices(new[] { BackToMainMenu })
            );
            
            if (selected is BackToMainMenu)
                return factory.Create<MainMenuView>();
        }
        
        return this;
    }
}