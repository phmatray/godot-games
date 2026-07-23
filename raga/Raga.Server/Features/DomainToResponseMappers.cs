using Raga.Server.Data.Models;

namespace Raga.Server.Features;

public static class DomainToResponseMappers
{
    public static ClanModel ToClanResponse(
        this Clan clan)
    {
        var target = new ClanModel
        {
            Id = clan.Id,
            Name = clan.Name,
            Description = clan.Description,
            MemberCount = clan.Members.Count,
            Score = clan.Members.Sum(m => m.TotalCurrency)
        };
        
        if (clan.Location != null)
        {
            target.Location = "Global";
        }
        
        return target;
    }
    
    public static PlayerModel ToPlayerResponse(
        this Player player)
    {
        var target = new PlayerModel
        {
            PlayerId = player.PlayerId,
            Level = player.Level,
            TotalPulls = player.TotalPulls,
            TotalCurrency = player.TotalCurrency,
            Achievements = { player.Achievements }
        };
        
        return target;
    }
    
    public static GachaItemModel ToGachaItemResponse(
        this GachaItem item)
    {
        var target = new GachaItemModel
        {
            Id = item.Id,
            Name = item.Name,
            Rarity = item.Rarity,
            Power = item.Power,
            Level = item.Level
        };
        
        return target;
    }
}