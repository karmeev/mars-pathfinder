using System.Collections.Concurrent;
using System.Threading.Channels;
using Grpc.Core;
using Pathfinder.Messages;
using SendMessageRequest = Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests.SendMessageRequest;

namespace Nasa.Pathfinder.Hubs;

public class MessageHub(Channel<SendMessageRequest> channel)
{
    private readonly SemaphoreSlim _limiter = new(10);
    private readonly ConcurrentQueue<SendMessageRequest> _responseQueue = new();
    private readonly ConcurrentDictionary<string, IServerStreamWriter<SendMessageResponse>> _streams = new();

    public void Start()
    {
        StartReceiver(channel, _responseQueue);
        for (var i = 0; i < 3; i++) StartConsumer(_responseQueue, CancellationToken.None);
    }

    public void Connect(string clientId, IServerStreamWriter<SendMessageResponse> stream)
    {
        _streams.TryAdd(clientId, stream);
    }

    public void Disconnect(string clientId)
    {
        _streams.TryRemove(clientId, out _);
    }

    private Task StartReceiver(Channel<SendMessageRequest> channel, ConcurrentQueue<SendMessageRequest> queue)
    {
        return Task.Run(async () =>
        {
            await foreach (var message in channel.Reader.ReadAllAsync())
            {
                await _limiter.WaitAsync();
                queue.Enqueue(message);
            }
        });
    }

    private Task StartConsumer(ConcurrentQueue<SendMessageRequest> queue, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
            {
                if (queue.IsEmpty)
                {
                    await Task.Delay(20, token);
                    continue;
                }

                if (queue.TryDequeue(out var message)) await NotifyClient(message);
            }
        }, token);
    }

    private async Task NotifyClient(SendMessageRequest message)
    {
        if (_streams.TryGetValue(message.ClientId, out var stream))
        {
            var reply = new SendMessageResponse
            {
                BotId = message.BotId,
                Message = message.Message,
                IsLost = message.IsLost,
                IsInvalidCommand = message.IsInvalidCommand,
                LastWords = message.LastWords
            };

            await stream.WriteAsync(reply);
        }
    }
}