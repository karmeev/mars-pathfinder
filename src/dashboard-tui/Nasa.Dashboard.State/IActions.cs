using Nasa.Dashboard.Model;

namespace Nasa.Dashboard.State;

public interface IAction { }

public record LoadBotsAction() : IAction;
public record LoadBotsActionResult(List<Bot> Bots) : IAction;
public record SelectBotAction(string BotId) : IAction;
public record SelectBotActionResult(Bot Bot) : IAction;
public record ResetSelectedBotAction(Bot Bot) : IAction;
public record ResetSelectedBotActionResult(Bot Bot) : IAction;
