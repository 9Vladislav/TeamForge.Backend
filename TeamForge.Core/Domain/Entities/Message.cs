using TeamForge.Core.Domain.Entities;

public class Message
{
    public int MessageId { get; set; }

    public int ChatId { get; set; }

    public int SenderId { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public Chat Chat { get; set; } = null!;

    public User Sender { get; set; } = null!;
}