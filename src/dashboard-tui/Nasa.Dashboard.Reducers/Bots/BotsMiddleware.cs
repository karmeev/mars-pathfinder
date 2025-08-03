using Nasa.Dashboard.Model;
using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.State;
using Nasa.Dashboard.Store;

namespace Nasa.Dashboard.Reducers.Bots;

public class BotsMiddleware(IDispatcher dispatcher)
{
    public async Task HandleAsync(IAction action)
    {
        switch (action)
        {
            case LoadBotsAction:
                //var bots = await _client.GetBotsAsync();
                var bots = new List<Bot>
                {
                    new Bot()
                    {
                        Id = "1",
                        Name = "Bot1",
                        Status = BotStatus.Available
                    }
                };
                dispatcher.Dispatch(new LoadBotsActionResult(bots));
                break;
            case SelectBotAction selectBotAction:
                //selectBotAction.BotId fetch by id
                var bot = new Bot()
                {
                    Id = "1",
                    Name = "Bot1",
                    Status = BotStatus.Available
                };
                dispatcher.Dispatch(new SelectBotActionResult(bot));
                break;
            case ResetSelectedBotAction resetSelectedBotAction:
                dispatcher.Dispatch(new ResetSelectedBotActionResult(resetSelectedBotAction.Bot));
                break;
        }
    }
}
