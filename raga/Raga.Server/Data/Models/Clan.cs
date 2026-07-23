namespace Raga.Server.Data.Models;

public class Clan
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Location { get; set; }
    public List<Player> Members { get; set; } = [];
}