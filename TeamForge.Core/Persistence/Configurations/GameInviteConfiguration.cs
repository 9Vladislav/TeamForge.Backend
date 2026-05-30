using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class GameInviteConfiguration : IEntityTypeConfiguration<GameInvite>
{
    public void Configure(EntityTypeBuilder<GameInvite> builder)
    {
        builder.HasKey(i => i.InviteId);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.HasOne(i => i.Sender)
            .WithMany()
            .HasForeignKey(i => i.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Receiver)
            .WithMany()
            .HasForeignKey(i => i.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(i => i.Game)
            .WithMany(g => g.GameInvites)
            .HasForeignKey(i => i.GameId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}