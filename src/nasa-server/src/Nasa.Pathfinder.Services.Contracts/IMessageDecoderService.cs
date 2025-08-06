using Nasa.Pathfinder.Domain.Interactions;

namespace Nasa.Pathfinder.Services.Contracts;

public interface IMessageDecoderService
{
    /// <summary>
    /// Encodes the bot's position and status into a message string.
    /// </summary>
    /// <param name="position">The current position of the bot.</param>
    /// <param name="isLost">Indicates whether the bot is lost.</param>
    /// <param name="isSave">Indicates whether the bot's position should be saved.</param>
    /// <returns>Encoded message representing the bot state.</returns>
    string EncodeBotMessage(Position position, bool isLost, bool isSave);
    
    /// <summary>
    /// Decodes a raw operator input string into a list of executable commands.
    /// </summary>
    /// <param name="text">The operator's input command string.</param>
    /// <returns>A read-only list of parsed operator commands.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the input is empty or exceeds 100 characters.
    /// </exception>
    IReadOnlyList<IOperatorCommand> DecodeOperatorMessage(string text);
}