namespace TeamForge.Core.Domain.Entities;

public class Friend
{
    public int FriendId { get; set; }

    public int UserId { get; set; }

    public int FriendUserId { get; set; }

    public User User { get; set; } = null!;

    public User FriendUser { get; set; } = null!;
}