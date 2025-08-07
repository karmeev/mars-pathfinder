using Nasa.Dashboard.Model.Messages;
using Nasa.Dashboard.State.Actions.ControlPanel;
using Nasa.Dashboard.State.Actions.System;
using Nasa.Dashboard.Store.Contracts;
using Nasa.Dashboard.View.Internal.Core;
using Nasa.Dashboard.View.Internal.Views.Components.Common;
using Spectre.Console;

namespace Nasa.Dashboard.View.Internal.Views;

internal class DrivingView(IViewFactory factory, IStore store) : IView
{
    private const string BackToMainMenu = "<-- Back to Main Menu";
    
    public IView Render()
    {
        var state = store.CurrentState;
        var panelState = state.ControlPanelState;

        if (state.BotState.SelectedBot is null)
        {
            Header.RenderHeader(state, () => store.Dispatch(new PingAction()));
            return DeadEnd();
        }

        if (!panelState.IsStreaming)
        {
            Header.RenderHeader(state, () => store.Dispatch(new PingAction()));
            store.Dispatch(new StreamingAction());
            return this;
        }

        if (!store.CurrentState.IsConnected)
        {
            return factory.Create<MainMenuView>();
        }
        
        Header.RenderHeader(state, () => store.Dispatch(new PingAction()));
        RenderChat(panelState.Messages);

        return PromptLoop();
    }

    private IView PromptLoop()
    {
        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine("Type your message ([grey]:back[/] to exit):");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || input.Trim() == ":back")
        {
            return ReturnToMainMenu();
        }

        var stateBeforeSend = store.CurrentState;
        var previousMessageCount = stateBeforeSend.ControlPanelState.Messages.Count;

        store.Dispatch(new SendOperatorMessageAction(
            new OperatorMessage { Text = input },
            stateBeforeSend.BotState.SelectedBot));
        
        return WaitForBotResponse();
    }
    
    private IView WaitForBotResponse()
    {
        while (store.CurrentState.ControlPanelState.Messages.LastOrDefault() is not BotMessage)
        {
            Thread.Sleep(10);
        }
        
        AnsiConsole.Clear();
        Header.RenderHeader(store.CurrentState, () => store.Dispatch(new PingAction()));
        RenderChat(store.CurrentState.ControlPanelState.Messages);
        return PromptLoop();
    }
    
    private void RenderChat(IReadOnlyList<IMessage> messages)
    {
        if (store.CurrentState.ControlPanelState.IsStreaming)
        {
            AnsiConsole.MarkupLine($"[grey]Current position:[/] [bold]Disconnected[/]");
        }
        
        AnsiConsole.MarkupLine("[underline]Chat:[/]");
        foreach (var msg in messages)
        {
            var prefix = msg is OperatorMessage ? "[green]You:[/]" : "[yellow]Bot:[/]";
            AnsiConsole.MarkupLine($"{prefix} {msg.Text}");
        }
        AnsiConsole.WriteLine();
    }
    
    private IView ReturnToMainMenu()
    {
        store.Dispatch(new ExitControlPanelAction());
        return factory.Create<MainMenuView>();
    }

    private IView DeadEnd()
    {
        AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Bot is [red]not[/] selected!")
                .AddChoices(new[] { BackToMainMenu })
        );
            
        return ReturnToMainMenu();
    }
}