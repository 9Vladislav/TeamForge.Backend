using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TeamForge.Core.Domain.Entities;

namespace TeamForge.Core.Persistence.Configurations;

public class ActivityPeriodConfiguration : IEntityTypeConfiguration<ActivityPeriod>
{
    public void Configure(EntityTypeBuilder<ActivityPeriod> builder)
    {
        builder.HasKey(ap => ap.ActivityPeriodId);

        builder.Property(ap => ap.DayOfWeek)
            .IsRequired();

        builder.Property(ap => ap.TimeFrom)
            .IsRequired();

        builder.Property(ap => ap.TimeTo)
            .IsRequired();

        builder.HasOne(ap => ap.User)
            .WithMany(u => u.ActivityPeriods)
            .HasForeignKey(ap => ap.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}