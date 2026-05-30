namespace TeamForge.Core.Domain.Entities;

public class Rating
{
    public int RatingId { get; set; }

    public int AuthorId { get; set; }

    public int ReceiverId { get; set; }

    public int InviteId { get; set; }

    public int Score { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Author { get; set; } = null!;

    public User Receiver { get; set; } = null!;

    public GameInvite Invite { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}