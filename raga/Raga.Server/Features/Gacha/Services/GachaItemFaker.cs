using Bogus;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Features.Gacha.Services;

public sealed class GachaItemFaker
    : IFakeDataGenerator<GachaItem>
{
    private readonly Faker<GachaItem> _faker;

    public GachaItemFaker()
    {
        _faker = new Faker<GachaItem>()
            .RuleFor(i => i.Id, f => f.Random.Guid().ToString())
            .RuleFor(i => i.Name, f => f.Commerce.ProductName())
            .RuleFor(i => i.Rarity, f => f.PickRandom("Common", "Uncommon", "Rare", "Epic", "Legendary"))
            .RuleFor(i => i.Power, f => f.Random.Int(10, 100))
            .RuleFor(i => i.Level, f => f.Random.Int(1, 10));
    }

    public GachaItem Generate()
    {
        return _faker.Generate();
    }
}