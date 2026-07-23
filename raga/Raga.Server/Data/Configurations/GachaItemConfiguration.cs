using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Raga.Server.Data.Models;

namespace Raga.Server.Data.Configurations;

public class GachaItemConfiguration : IEntityTypeConfiguration<GachaItem>
{
    public void Configure(EntityTypeBuilder<GachaItem> builder)
    {
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(g => g.Rarity)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(g => g.Power)
            .IsRequired();
        
        builder.Property(g => g.Level)
            .IsRequired();
        
        builder.HasOne(g => g.Player)
            .WithMany(p => p.Inventory)
            .HasForeignKey(g => g.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}