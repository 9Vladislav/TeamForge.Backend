using TeamForge.Core.Domain.Enums;

namespace TeamForge.Core.Domain.Entities;

public class FriendRequest
{
    public int FriendRequestId { get; set; }

    public int SenderId { get; set; }

    public int ReceiverId { get; set; }

    public FriendRequestStatus Status { get; set; } =
        FriendRequestStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User Sender { get; set; } = null!;

    public User Receiver { get; set; } = null!;
}