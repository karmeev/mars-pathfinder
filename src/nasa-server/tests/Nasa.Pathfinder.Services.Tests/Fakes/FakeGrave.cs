using Bogus;
using Nasa.Pathfinder.Domain.Entities.World;
using Nasa.Pathfinder.Domain.World;

namespace Nasa.Pathfinder.Services.Tests.Fakes;

public static class FakeGrave
{
    public static Grave Make(string mapId)
    {
        return new Faker<Grave>()
            .RuleFor(x => x.Id, faker => faker.Database.Random.Uuid().ToString())
            .RuleFor(x => x.ETag, faker => faker.Random.Guid())
            .RuleFor(x => x.MapId, mapId)
            .RuleFor(x => x.Value, _ =>
                new Faker<Position>()
                    .RuleFor(x => x.X, faker => faker.Random.Int())
                    .RuleFor(x => x.Y, faker => faker.Random.Int())
                    .RuleFor(x => x.Direction, faker => faker.Random.Enum<Direction>()));
    }
}