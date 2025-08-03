namespace Nasa.Dashboard.State.Actions.System;

public record PingAction : IAction;

public record PingActionResult(bool IsSuccessful) : IAction;