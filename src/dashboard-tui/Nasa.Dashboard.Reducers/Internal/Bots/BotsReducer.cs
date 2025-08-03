using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.State.States;

namespace Nasa.Dashboard.Reducers.Internal.Bots;

internal static class BotsReducer
{
    public static AppState Reduce(AppState state, IAction action)
    {
        var botState = state.BotState;
        return action switch
        {
            LoadBotsAction => state with
            {
                BotState = botState with
                {
                    Bots = new List<Bot>(),
                    IsLoading = true
                }
            },
            LoadBotsActionResult loadBotsActionResult => state with
            {
                BotState = botState with
                {
                    Bots = loadBotsActionResult.Bots,
                    IsLoading = false
                }
            },
            SelectBotAction => state with
            {
                BotState = botState with
                {
                    IsLoading = true
                }
            },
            SelectBotActionResult selectBotActionResult => state with
            {
                BotState = botState with
                {
                    IsLoading = false,
                    SelectedBot = selectBotActionResult.Bot
                }
            },
            ResetSelectedBotAction => state with
            {
                BotState = botState with
                {
                    IsLoading = true
                }
            },
            ResetSelectedBotActionResult => state with
            {
                BotState = botState with
                {
                    IsLoading = false,
                    SelectedBot = null
                }
            },

            _ => state
        };
    }
}