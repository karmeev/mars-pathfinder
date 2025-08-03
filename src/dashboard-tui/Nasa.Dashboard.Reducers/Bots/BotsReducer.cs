using Nasa.Dashboard.Model;
using Nasa.Dashboard.State;

namespace Nasa.Dashboard.Reducers.Bots;

public static class BotsReducer
{
    public static AppState Reduce(AppState state, IAction action)
    {
        return action switch
        {
            LoadBotsAction => state with
            {
                Bots = new List<Bot>(),
                IsLoading = true
            },
            LoadBotsActionResult loadBotsActionResult => state with
            {
                Bots = loadBotsActionResult.Bots,
                IsLoading = false
            },
            SelectBotAction selectBotAction => state with
            {
                IsLoading = true,
            },
            SelectBotActionResult selectBotActionResult => state with
            {
                IsLoading = false,
                SelectedBot = selectBotActionResult.Bot
            },
            ResetSelectedBotAction resetSelectedBotAction => state with
            {
                IsLoading = true,
            },
            ResetSelectedBotActionResult resetSelectedBotActionResult => state with
            {
                IsLoading = false,
                SelectedBot = null
            },
            _ => state
        };
    }
}