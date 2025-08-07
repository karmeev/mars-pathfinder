using Bogus;
using Nasa.Pathfinder.Domain.Interactions;
using Nasa.Pathfinder.Domain.World;
using Nasa.Pathfinder.Services.Internal;
using Nasa.Pathfinder.Tests;
using NUnit.Framework;

namespace Nasa.Pathfinder.Services.Tests;

[TestFixture]
public class MessageDecoderServiceTests
{
    [Test]
    public void DecodeOperatorMessage_ProvidedTextWithInstruction_ShouldReturnsCommands()
    {
        const string input = "FRFFL";
        TestRunner<MessageDecoderService, Stack<IOperatorCommand>>
            .Arrange(() => new MessageDecoderService())
            .Act(sut => sut.DecodeOperatorMessage(input))
            .Assert(result =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(5));
                Assert.That(ConvertToString(result), Is.EqualTo(input));
            });
    }

    [Test]
    public void DecodeOperatorMessage_ProvidedIncorrectText_ShouldReturnUnknownCommands()
    {
        const string input = "gHrqRRRLFFF";
        TestRunner<MessageDecoderService, Stack<IOperatorCommand>>
            .Arrange(() => new MessageDecoderService())
            .Act(sut => sut.DecodeOperatorMessage(input))
            .Assert(result =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Where(x => x is UnknownCommand).Count, Is.EqualTo(4));
            });
    }

    [Test]
    public void DecodeOperatorMessage_ProvidedLongText_ShouldThrowInvalidOperationException()
    {
        var command = new Faker().Lorem.Sentence(200);
        TestRunner<MessageDecoderService, IReadOnlyList<IOperatorCommand>>
            .Arrange(() => new MessageDecoderService())
            .ActAndAssert(sut =>
            {
                Assert.Throws<InvalidOperationException>(() => sut.DecodeOperatorMessage(command));
            });
    }

    [Test]
    public void DecodeOperatorMessage_ProvidedEmptyText_ShouldThrowInvalidOperationException()
    {
        var command = string.Empty;
        TestRunner<MessageDecoderService, IReadOnlyList<IOperatorCommand>>
            .Arrange(() => new MessageDecoderService())
            .ActAndAssert(sut =>
            {
                Assert.Throws<InvalidOperationException>(() => sut.DecodeOperatorMessage(command));
            });
    }

    [Test]
    public void EncodeBotMessage_ProvidedPosition_ShouldReturnTextRepresentation()
    {
        var position = new Position
        {
            X = 1,
            Y = 2,
            Direction = Direction.N
        };

        TestRunner<MessageDecoderService, string>
            .Arrange(() => new MessageDecoderService())
            .Act(sut => sut.EncodeBotMessage(position, false))
            .Assert(result =>
            {
                Assert.That(result, Is.Not.Null.And.Not.Empty);
                Assert.That(result, Is.EqualTo("1 2 N"));
            });
    }

    [Test]
    public void EncodeBotMessage_ProvidedLostPosition_ShouldReturnTextRepresentation()
    {
        var position = new Position
        {
            X = 1,
            Y = 2,
            Direction = Direction.N
        };

        TestRunner<MessageDecoderService, string>
            .Arrange(() => new MessageDecoderService())
            .Act(sut => sut.EncodeBotMessage(position, true))
            .Assert(result =>
            {
                Assert.That(result, Is.Not.Null.And.Not.Empty);
                Assert.That(result, Is.EqualTo("1 2 N LOST"));
            });
    }

    private string ConvertToString(Stack<IOperatorCommand> commands)
    {
        var output = string.Empty;
        foreach (var command in commands) output += command.Shorthand;

        return output;
    }
}