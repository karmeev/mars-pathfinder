namespace Nasa.Pathfinder.Domain.Messages;

public class OperatorMessage : IMessage
{
    public string ClientId { get; set; }
    public string BotId { get; set; }
    public string Text { get; set; }
}