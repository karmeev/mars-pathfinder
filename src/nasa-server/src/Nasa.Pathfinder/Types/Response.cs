using Grpc.Core;
using Pathfinder.Messages;

namespace Nasa.Pathfinder.Types;

public static class Response
{
    public static T InvalidArgument<T>(string message)
    {
        throw new RpcException(new Status(StatusCode.InvalidArgument, message));
    }
    
    public static GetBotsResponse MapToGetBotsResponse(IEnumerable<Nasa.Pathfinder.Domain.Bots.Bot> bots)
    {
        var response = new GetBotsResponse();
        foreach (var bot in bots)
            response.Bots.Add(new Bot
            {
                Id = bot.Id,
                Name = bot.Name,
                Status = bot.Status.ToString(),
                Position = new Position
                {
                    X = bot.Position.X,
                    Y = bot.Position.Y,
                    Direction = bot.Position.Direction.ToString(),
                }
            });

        return response;
    }
    
    public static SelectBotResponse MapToSelectBotResponse(Nasa.Pathfinder.Domain.Bots.Bot bot)
    {
        return new SelectBotResponse
        {
            Bot = new Bot
            {
                Id = bot.Id,
                Name = bot.Name,
                Status = bot.Status.ToString()
            }
        };
    }
    
    public static ResetBotResponse MapToResetBotResponse(Nasa.Pathfinder.Domain.Bots.Bot bot)
    {
        var response = new ResetBotResponse
        {
            Bot = new Bot
            {
                Id = bot.Id,
                Name = bot.Name,
                Status = bot.Status.ToString()
            }
        };
        return response;
    }
}