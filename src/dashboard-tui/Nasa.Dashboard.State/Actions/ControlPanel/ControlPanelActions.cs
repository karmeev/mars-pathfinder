using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.State.Actions.ControlPanel;

public record StreamingAction : IAction;

public record StreamingActionResult : IAction;

public record StreamingErrorAction(Exception Exception) : IAction;

public record BotMessageReceivedAction(IMessage Message) : IAction;

public record BotMessageReceivedActionResult(IMessage Message) : IAction;

public record BotMessageErrorReceivedActionResult(IMessage Message) : IAction;

public record SendOperatorMessageAction(IMessage Message) : IAction;

public record SendOperatorMessageActionResult(IMessage Message) : IAction;