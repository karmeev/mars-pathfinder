using Nasa.Dashboard.State.Actions;
using Nasa.Dashboard.Store.Contracts;

namespace Nasa.Dashboard.Reducers.Internal.Bots;

internal class BotsMiddleware(IDispatcher dispatcher, IBotsService service)
{
    public async Task HandleAsync(IAction action)
    {
        switch (action)
        {
            case LoadBotsAction:
                var bots = await service.GetBotsAsync();
                dispatcher.Dispatch(new LoadBotsActionResult(bots));
                break;
            case SelectBotAction selectBotAction:
                var bot = await service.SelectBotAsync(selectBotAction.BotId);
                dispatcher.Dispatch(new SelectBotActionResult(bot));
                break;
            case ResetSelectedBotAction resetSelectedBotAction:
                //TODO: if resetting is not successful what's next?
                await service.ResetBotAsync(resetSelectedBotAction.Bot);
                dispatcher.Dispatch(new ResetSelectedBotActionResult(resetSelectedBotAction.Bot));
                break;
        }
    }
}
