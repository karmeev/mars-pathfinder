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

        if (state.SelectedBot is not null)
        {
            var statusColor = state.SelectedBot.Status switch
            {
                BotStatus.Available => "green",
                BotStatus.Acquired  => "yellow",
                BotStatus.Dead      => "red",
                _ => "grey"
            };

            AnsiConsole.MarkupLine($"[grey]Selected Bot:[/] [bold {statusColor}]{state.SelectedBot.Name} ({state.SelectedBot.Status})[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[grey]Selected Bot:[/] Not selected");
        }
        AnsiConsole.WriteLine();
    }
}