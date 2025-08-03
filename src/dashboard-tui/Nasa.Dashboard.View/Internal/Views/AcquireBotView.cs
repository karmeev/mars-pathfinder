using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views.Render;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class AcquireBotView(IViewFactory factory, IStore store) : IView
{
    public IView Render()
    {
        var state = store.CurrentState;
        
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
        
        if (state.IsLoading)
        {
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            Thread.Sleep(1000); // Optional: small delay for UX
            return this;       // Re-render view until loading is done
        }
        
        if (!state.Bots.Any())
        {
            store.Dispatch(new LoadBotsAction());
            AnsiConsole.MarkupLine("[bold underline yellow]Fetching bots...[/]");
            Thread.Sleep(1000); // Optional: small delay for UX
            return this;       // Re-render view until loading is done
        }
        
        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Status");
        
        foreach (var bot in state.Bots)
        {
            var statusColor = bot.Status switch
            {
                BotStatus.Available => "green",
                BotStatus.Acquired  => "yellow",
                BotStatus.Dead      => "red",
                _ => "grey"
            };

            table.AddRow(bot.Name, $"[{statusColor}]{bot.Status}[/]");
        }
        
        AnsiConsole.Write(table);
        
        var availableBots = store.CurrentState.Bots
            .Where(b => b.Status == BotStatus.Available)
            .Select(b => b.Name)
            .ToList();

        if (availableBots.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No bots available. Press any key to go back.[/]");
            Console.ReadKey();
            return factory.Create<MainMenuView>();
        }

        var selectedName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select a bot to [green]acquire[/]:")
                .AddChoices(availableBots));
        
        var botId = state.Bots.First(b => b.Name == selectedName).Id;
        store.Dispatch(new SelectBotAction(botId));
        return this;
    }
}