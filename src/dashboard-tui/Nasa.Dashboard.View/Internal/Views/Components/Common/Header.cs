using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views.Components.Common;

internal static class Header
{
    public static void RenderHeader(AppState state)
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(new FigletText("Nasa Space Program").Centered().Color(Color.Aqua));

        if (state.IsConnected)
        {
            AnsiConsole.MarkupLine($"[grey]Connection status:[/] [bold green]Connected[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[grey]Connection status:[/] [bold red]Disconnected[/]");
        }
        
        if (state.BotState.SelectedBot is not null)
        {
            var statusColor = state.BotState.SelectedBot.Status switch
            {
                BotStatus.Available => "green",
                BotStatus.Acquired  => "yellow",
                BotStatus.Dead      => "red",
                _ => "grey"
            };

            AnsiConsole.MarkupLine($"[grey]Selected Bot:[/] [bold {statusColor}]{state.BotState.SelectedBot.Name} " +
                                   $"({state.BotState.SelectedBot.Status})[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[grey]Selected Bot:[/] Not selected");
        }
        AnsiConsole.WriteLine();
    }
}