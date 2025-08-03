using Nasa.Dashboard.Model;
using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.State.Actions;

public interface IAction { }

public record LoadBotsAction() : IAction;

public record LoadBotsActionResult(IEnumerable<Bot> Bots) : IAction;

public record SelectBotAction(string BotId) : IAction;

public record SelectBotActionResult(Bot Bot) : IAction;

public record ResetSelectedBotAction(Bot Bot) : IAction;

public record ResetSelectedBotActionResult(Bot Bot) : IAction;
