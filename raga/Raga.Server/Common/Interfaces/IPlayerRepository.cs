using Raga.Server.Data.Models;

namespace Raga.Server.Common.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetPlayerAsync(string playerId);
    Task<Player> AddPlayerAsync(Player player);
    Task<Player> UpdatePlayerAsync(Player player);
    Task<List<GachaItem>> GetPlayerInventoryAsync(string playerId);
}
