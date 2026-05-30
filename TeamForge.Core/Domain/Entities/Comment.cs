namespace TeamForge.Core.Domain.Entities;

public class Comment
{
    public int CommentId { get; set; }

    public int RatingId { get; set; }

    public int AuthorId { get; set; }

    public int ReceiverId { get; set; }

    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Rating Rating { get; set; } = null!;

    public User Author { get; set; } = null!;

    public User Receiver { get; set; } = null!;
}