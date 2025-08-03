using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Dashboard.Clients.Contracts;
using Nasa.Dashboard.Model.Bots;
using Pathfinder.Messages;
using Pathfinder.Proto;
using Bot = Nasa.Dashboard.Model.Bots.Bot;

namespace Nasa.Dashboard.Clients.Internal.Pathfinder;

internal class PathfinderClient(PathfinderService.PathfinderServiceClient grpcService) : IPathfinderClient
{
    public async Task<bool> PingAsync()
    {
        var response = await grpcService.PingAsync(new Empty(), GetHeader());
        return response.IsSuccessful;
    }

    public async Task<IEnumerable<Bot>> GetBotsAsync()
    {
        var response = await grpcService.GetBotsAsync(new Empty(), GetHeader());

        return response.Bots.Select(bot => new Bot
        {
            Id = bot.Id, 
            Name = bot.Name, 
            Status = System.Enum.Parse<BotStatus>(bot.Status),
        }).ToList();
    }

    public async Task<Bot> SelectBotAsync(string botId)
    {
        var request = new SelectBotRequest
        {
            BotId = botId
        };
        var response = await grpcService.SelectBotAsync(request, GetHeader());
        return new Bot
        {
            Id = response.Bot.Id,
            Name = response.Bot.Name,
            Status = System.Enum.Parse<BotStatus>(response.Bot.Status)
        };
    }

    public async Task<bool> ResetBotAsync(string botId)
    {
        var request = new ResetBotRequest
        {
            BotId = botId
        };
        var response = await grpcService.ResetBotAsync(request, GetHeader());
        return true;
    }

    private Metadata GetHeader()
    {
        return new Metadata
        {
            { "TraceId", Activity.Current?.TraceId.ToString() }
        };
    }
}