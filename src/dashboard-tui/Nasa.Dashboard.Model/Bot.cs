using Nasa.Dashboard.Model.Bots;

namespace Nasa.Dashboard.Model;

public class Bot
{
    public string Id { get; set; }
    public string Name { get; set; }
    public BotStatus Status { get; set; }
}