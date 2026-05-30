using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class UserGameConfiguration : IEntityTypeConfiguration<UserGame>
{
    public void Configure(EntityTypeBuilder<UserGame> builder)
    {
        builder.HasKey(ug => ug.UserGameId);

        builder.Property(ug => ug.SkillLevel)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(ug => ug.PlaystyleDescription)
            .HasMaxLength(1000);

        builder.HasOne(ug => ug.User)
            .WithMany(u => u.UserGames)
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ug => ug.Game)
            .WithMany(g => g.UserGames)
            .HasForeignKey(ug => ug.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ug => new { ug.UserId, ug.GameId })
            .IsUnique();
    }
}