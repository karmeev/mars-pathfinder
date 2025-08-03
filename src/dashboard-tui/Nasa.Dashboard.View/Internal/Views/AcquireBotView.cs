using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views.Components.Bots;
using Nasa.Dashboard.View.Internal.Views.Components.Common;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class AcquireBotView(IViewFactory factory, IStore store) : IView
{
    private const string BackToMainMenu = "<-- Back to Main Menu";
    
    public IView Render()
    {
        var state = store.CurrentState;
        
        if (state.IsLoading)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            Thread.Sleep(5);
            return this;
        }
        
        Header.RenderHeader(state);
        
        if (state.SelectedBot is not null)
        {
            AnsiConsole.MarkupLine($"[bold green]Bot '{state.SelectedBot.Name}' is already selected.[/]");
    
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices("Keep it", "Select another bot")
            );

            if (choice == "Keep it")
            {
                return factory.Create<MainMenuView>();
            }
            
            store.Dispatch(new ResetSelectedBotAction(state.SelectedBot));
            return this;
        }
        
        if (!state.Bots.Any())
        {
            store.Dispatch(new LoadBotsAction());
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            return this;
        }
        
        BotsTable.RenderTable(state);
        
        var choices = store.CurrentState.Bots
            .Where(b => b.Status == BotStatus.Available)
            .Select(b => b.Name)
            .ToList();

        if (choices.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No bots available. Press any key to go back.[/]");
            Console.ReadKey();
            return factory.Create<MainMenuView>();
        }

        choices.Add(BackToMainMenu);
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a bot to [green]acquire[/]:")
                .AddChoices(choices));
        
        if (selected is BackToMainMenu)
            return factory.Create<MainMenuView>();
        
        var botId = state.Bots.First(b => b.Name == selected).Id;
        store.Dispatch(new SelectBotAction(botId));
        return this;
    }
}