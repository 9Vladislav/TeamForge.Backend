using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class FriendConfiguration : IEntityTypeConfiguration<Friend>
{
    public void Configure(EntityTypeBuilder<Friend> builder)
    {
        builder.HasKey(f => f.FriendId);

        builder.HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.FriendUser)
            .WithMany()
            .HasForeignKey(f => f.FriendUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(f => new { f.UserId, f.FriendUserId })
            .IsUnique();
    }
}