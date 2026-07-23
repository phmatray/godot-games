using Microsoft.EntityFrameworkCore;
using Raga.Server.Common.Interfaces;
using Raga.Server.Data.Models;

namespace Raga.Server.Data.Repositories;

public class GachaItemRepository(GachaContext context)
    : IGachaItemRepository
{
    public async Task<GachaItem> AddGachaItemAsync(GachaItem item)
    {
        context.GachaItems.Add(item);
        await context.SaveChangesAsync();
        return item;
    }
}