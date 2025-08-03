using Nasa.Dashboard.Model.Messages;
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
        
        if (!store.CurrentState.IsConnected)
        {
            return factory.Create<MainMenuView>();
        }
        
        var panelState = state.ControlPanelState;
        
        Header.RenderHeader(state);
        
        if (state.BotState.SelectedBot is null)
        {
            var selected = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Bot is [red]not[/] selected!")
                    .AddChoices(new[] { BackToMainMenu })
            );
            
            if (selected is BackToMainMenu)
                return factory.Create<MainMenuView>();
        }
        
        AnsiConsole.MarkupLine("[underline]Chat:[/]");
        foreach (var msg in panelState.Messages.TakeLast(10))
        {
            var prefix = msg.GetType() == typeof(OperatorMessage) ? "[green]You:[/]" : "[yellow]Bot:[/]";
            AnsiConsole.MarkupLine($"{prefix} {msg.Text}");
        }
        
        AnsiConsole.WriteLine();
        
        var input = AnsiConsole.Ask<string>("Type your message ([grey]:back[/] to exit):");

        if (string.IsNullOrWhiteSpace(input) || input.Trim() == ":back")
        {
            return factory.Create<MainMenuView>();
        }
        
        return this;
    }
}