using Nasa.Pathfinder.Domain.Bots;
using Nasa.Pathfinder.Domain.World;
using Pathfinder.Messages;
using Bot = Nasa.Pathfinder.Domain.Entities.Bots.Bot;
using Position = Nasa.Pathfinder.Domain.World.Position;


namespace Nasa.Pathfinder.Types;

public static class Request
{
    public static IEnumerable<Bot> MapFromCreateWorldRequest(CreateWorldRequest request)
    {
        return request.Bots.Select(bot => new Bot
        {
            Id = Guid.NewGuid().ToString(),
            ETag = Guid.NewGuid(),
            Name = bot.Name,
            Status = BotStatus.Available,
            Position = new Position
            {
                X = bot.Position.X,
                Y = bot.Position.Y,
                Direction = Enum.Parse<Direction>(bot.Position.Direction),
            }
        }).ToList();
    }
}