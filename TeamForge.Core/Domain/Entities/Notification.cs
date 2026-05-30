using TeamForge.Core.Domain.Enums;

namespace TeamForge.Core.Domain.Entities;

public class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public NotificationType Type { get; set; }

    public string MessageText { get; set; } = string.Empty;

    public bool IsRead { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}