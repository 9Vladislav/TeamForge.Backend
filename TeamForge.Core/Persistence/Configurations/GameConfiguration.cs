using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(g => g.GameId);

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.HasIndex(g => g.Name)
            .IsUnique();

        builder.Property(g => g.ImageUrl)
            .HasMaxLength(500);
    }
}