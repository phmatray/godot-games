namespace Raga.Server.Data.Models;

public class Player
{
    public string PlayerId { get; set; }
    public int Level { get; set; } = 1;
    public int TotalPulls { get; set; }
    public int TotalCurrency { get; set; } = 100;
    public DateTime LastDailyRewardClaim { get; set; }
    public List<GachaItem> Inventory { get; set; } = [];
    public List<string> Achievements { get; set; } = [];
    public string? ClanId { get; set; }
    public Clan? Clan { get; set; }
}