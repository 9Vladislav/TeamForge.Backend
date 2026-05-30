using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class ChatConfiguration : IEntityTypeConfiguration<Chat>
{
    public void Configure(EntityTypeBuilder<Chat> builder)
    {
        builder.HasKey(c => c.ChatId);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasOne(c => c.FirstUser)
            .WithMany()
            .HasForeignKey(c => c.FirstUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.SecondUser)
            .WithMany()
            .HasForeignKey(c => c.SecondUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new
        {
            c.FirstUserId,
            c.SecondUserId
        });
    }
}