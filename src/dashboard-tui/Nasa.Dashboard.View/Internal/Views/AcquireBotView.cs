using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.Actions.Bots;
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
        
        if (state.BotState.IsLoading)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            Thread.Sleep(5);
            return this;
        }
        
        Header.RenderHeader(state);
        
        if (state.BotState.SelectedBot is not null)
        {
            AnsiConsole.MarkupLine($"[bold green]Bot '{state.BotState.SelectedBot.Name}' is already selected.[/]");
    
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices("Keep it", "Select another bot")
            );

            if (choice == "Keep it")
            {
                return factory.Create<MainMenuView>();
            }
            
            store.Dispatch(new ResetSelectedBotAction(state.BotState.SelectedBot));
            return this;
        }
        
        if (!state.BotState.Bots.Any())
        {
            store.Dispatch(new LoadBotsAction());
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            return this;
        }
        
        BotsTable.RenderTable(state);
        
        var choices = store.CurrentState.BotState.Bots?
            .Where(b => b.Status == BotStatus.Available)
            .Select(b => b.Name)
            .ToList();

        if (choices is null || choices.Count == 0)
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
        
        var botId = state.BotState.Bots.First(b => b.Name == selected).Id;
        store.Dispatch(new SelectBotAction(botId));
        return this;
    }
}