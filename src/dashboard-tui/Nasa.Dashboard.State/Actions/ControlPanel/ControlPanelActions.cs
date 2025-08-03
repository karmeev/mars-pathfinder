using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.Model.Messages;

namespace Nasa.Dashboard.State.Actions.ControlPanel;

public record StreamingAction : IAction;

public record StreamingActionResult(bool IsStreaming) : IAction;

public record StreamingErrorAction(Exception Exception) : IAction;

public record BotMessageReceivedActionResult(IMessage Message) : IAction;

public record BotMessageErrorReceivedActionResult(IMessage Message) : IAction;

public record SendOperatorMessageAction(IMessage Message, Bot Bot) : IAction;

public record SendOperatorMessageActionResult(IMessage Message) : IAction;

public record ExitControlPanelAction : IAction;

public record CleanMessages : IAction;

public record CleanMessagesResult : IAction;