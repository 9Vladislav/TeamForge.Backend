using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.RatingId);

        builder.Property(r => r.Score)
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.HasOne(r => r.Author)
            .WithMany()
            .HasForeignKey(r => r.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Receiver)
            .WithMany()
            .HasForeignKey(r => r.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Invite)
            .WithMany()
            .HasForeignKey(r => r.InviteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.AuthorId, r.ReceiverId, r.InviteId })
            .IsUnique();
    }
}