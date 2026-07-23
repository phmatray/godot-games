using Microsoft.EntityFrameworkCore;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Data.Repositories;

public class ClanRepository(
    GachaContext context)
    : IClanRepository
{
    public async Task<Clan> CreateClanAsync(Clan clan)
    {
        context.Clans.Add(clan);
        await context.SaveChangesAsync();
        return clan;
    }

    public async Task<Clan?> GetClanByIdAsync(string clanId)
    {
        return await context.Clans
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == clanId);
    }

    public async Task<bool> JoinClanAsync(string playerId, string clanId)
    {
        var player = await context.Player.FindAsync(playerId);
        if (player == null || player.ClanId != null)
        {
            return false;
        }

        player.ClanId = clanId;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LeaveClanAsync(string playerId, string clanId)
    {
        var player = await context.Player.FindAsync(playerId);
        if (player == null || player.ClanId != clanId)
        {
            return false;
        }

        player.ClanId = null;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Player>> GetClanMembersAsync(string clanId)
    {
        var clan = await context.Clans
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Id == clanId);
        
        return clan?.Members ?? [];
    }

    public async Task<List<Clan>> GetClansByScoreAsync(string? location)
    {
        var query = context.Clans
            .Include(c => c.Members)
            .AsQueryable();
        
        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(c => c.Location == location);
        }

        return await query
            .OrderByDescending(c => c.Members.Sum(m => m.TotalCurrency))
            .ToListAsync();
    }

    public async Task<bool> ClanNameExistsAsync(string name)
    {
        return await context.Clans
            .AnyAsync(c => c.Name == name);
    }

    public async Task<int> GetClanMemberCountAsync(string clanId)
    {
        return await context.Player
            .CountAsync(p => p.ClanId == clanId);
    }
}