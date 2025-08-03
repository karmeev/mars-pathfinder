using System.Diagnostics;
using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Nasa.Dashboard.Clients.Contracts;
using Nasa.Dashboard.Model.Bots;
using Nasa.Dashboard.Model.Messages;
using Pathfinder.Messages;
using Pathfinder.Proto;
using Bot = Nasa.Dashboard.Model.Bots.Bot;

namespace Nasa.Dashboard.Clients.Internal.Pathfinder;

internal class PathfinderClient(PathfinderService.PathfinderServiceClient grpcService) : IPathfinderClient
{
    private AsyncDuplexStreamingCall<SendMessageRequest, SendMessageResponse>? _stream;
    
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

    public void StartChat()
    {
        _stream = grpcService.SendMessage(GetHeader());
    }
    
    public bool IsAlreadyInChat() => _stream != null;

    public async Task SendMessageAsync(string botId, string text)
    {
        if (_stream == null)
            throw new InvalidOperationException("Stream not started");

        await _stream.RequestStream.WriteAsync(new SendMessageRequest
        {
            BotId = botId,
            Message = text
        });
    }
    
    public async IAsyncEnumerable<IMessage> GetIncomingMessagesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (_stream == null)
            yield break;

        var stream = _stream.ResponseStream;

        while (await stream.MoveNext(cancellationToken))
        {
            var message = stream.Current;
            yield return ToMessage(message);
        }
    }
    
    public async Task StopAsync()
    {
        if (_stream != null)
        {
            await _stream.RequestStream.CompleteAsync();
            _stream.Dispose();
            _stream = null;
        }
    }

    private static IMessage ToMessage(SendMessageResponse response)
    {
        // Example conversion. Customize as needed.
        return new BotMessage
        {
            Text = response.Message,
            //BotId = response.BotId,
        };
    }

    private Metadata GetHeader()
    {
        return new Metadata
        {
            { "TraceId", Activity.Current?.TraceId.ToString() }
        };
    }
}