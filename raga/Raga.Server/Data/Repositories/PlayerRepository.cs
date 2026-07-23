using Microsoft.EntityFrameworkCore;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Data.Repositories;

public class PlayerRepository(
    GachaContext context)
    : IPlayerRepository
{
    public async Task<Player?> GetPlayerAsync(string playerId)
    {
        return await context.Player
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.PlayerId == playerId);
    }

    public async Task<Player> AddPlayerAsync(Player player)
    {
        context.Player.Add(player);
        await context.SaveChangesAsync();
        return player;
    }

    public async Task<Player> UpdatePlayerAsync(Player player)
    {
        context.Player.Update(player);
        await context.SaveChangesAsync();
        return player;
    }

    public async Task<List<GachaItem>> GetPlayerInventoryAsync(string playerId)
    {
        return await context.GachaItems
            .Where(i => i.PlayerId == playerId)
            .ToListAsync();
    }
}