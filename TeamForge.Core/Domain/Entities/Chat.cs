namespace TeamForge.Core.Domain.Entities;

public class Chat
{
    public int ChatId { get; set; }

    public int FirstUserId { get; set; }

    public int SecondUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User FirstUser { get; set; } = null!;

    public User SecondUser { get; set; } = null!;

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}