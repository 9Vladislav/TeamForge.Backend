namespace TeamForge.Core.Domain.Entities;

public class ActivityPeriod
{
    public int ActivityPeriodId { get; set; }

    public int UserId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly TimeFrom { get; set; }

    public TimeOnly TimeTo { get; set; }

    public User User { get; set; } = null!;
}