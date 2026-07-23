using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Server.Data.Models;

namespace Raga.Server.Data.Configurations;

public class ClanConfiguration : IEntityTypeConfiguration<Clan>
{
    public void Configure(EntityTypeBuilder<Clan> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Description)
            .HasMaxLength(500);
        
        builder.Property(c => c.Location)
            .IsRequired(false)
            .HasMaxLength(100);
        
        builder.HasMany(c => c.Members)
            .WithOne(p => p.Clan)
            .HasForeignKey(p => p.ClanId);
    }
}