using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.CommentId);

        builder.Property(c => c.Text)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.HasOne(c => c.Rating)
            .WithMany(r => r.Comments)
            .HasForeignKey(c => c.RatingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Author)
            .WithMany()
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Receiver)
            .WithMany()
            .HasForeignKey(c => c.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}