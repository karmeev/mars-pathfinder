using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views.Components.Bots;

internal static class BotsTable
{
    public static void RenderTable(AppState state)
    {
        var table = new Table();
        table.AddColumn("Name");
        table.AddColumn("Status");
        
        foreach (var bot in state.BotState.Bots)
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
    }
}