using TeamForge.Core.Domain.Enums;

namespace TeamForge.Core.Domain.Entities;

public class GameInvite
{
    public int InviteId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public int GameId { get; set; }

    public InviteStatus Status { get; set; } = InviteStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Sender { get; set; } = null!;

    public User Receiver { get; set; } = null!;

    public Game Game { get; set; } = null!;
}