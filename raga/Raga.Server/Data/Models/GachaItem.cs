namespace Raga.Server.Data.Models;

public class GachaItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Rarity { get; set; }
    public int Power { get; set; }
    public int Level { get; set; }
    public string PlayerId { get; set; }
    public Player Player { get; set; }
}