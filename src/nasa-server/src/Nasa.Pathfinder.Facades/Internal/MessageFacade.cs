using Nasa.Pathfinder.Data.Contracts.Repositories;
using Nasa.Pathfinder.Domain.Messages;
using Nasa.Pathfinder.Facades.Contracts;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc;
using Nasa.Pathfinder.Infrastructure.Contracts.Grpc.Requests;
using Nasa.Pathfinder.Services.Contracts;

namespace Nasa.Pathfinder.Facades.Internal;

internal class MessageFacade(
    IBotRepository repository,
    IMessageDecoderService messageDecoder,
    IWorldMapService worldMap,
    IOperatorStream stream) : IMessageFacade
{
    public async Task ReceiveMessageAsync(IMessage message, CancellationToken ct = default)
    {
        if (message is not OperatorMessage operatorMessage)
            throw new InvalidCastException();
        
        var command = messageDecoder.DecodeOperatorMessage(operatorMessage.Text);
        
        var bot = await repository.GetAsync(operatorMessage.BotId, ct);

        var desiredPosition = worldMap.CalculateDesiredPosition(bot.Position, command.OperatorCommands);
        
        var funerals = await worldMap.GetFuneralsAsync(ct);
        var isTrap = funerals.Contains(desiredPosition);
        
        var isAccessible = await worldMap.TryReachPosition(desiredPosition, ct);
        var isLost = false;
        var isSave = false;
        
        if (isTrap && isAccessible)
        {
            isSave = true;
        }
        else
        {
            if (!isAccessible)
            {
                isLost = true;
            }

            await worldMap.ChangeBotPositionAsync(desiredPosition, ct);
        }
        
        var notificationText = messageDecoder.EncodeBotMessage(desiredPosition, isLost, isSave);
        var request = new SendMessageRequest(bot.Id, operatorMessage.ClientId, notificationText, isLost);
        stream.SendMessage(request);
    }
}