using TeamForge.Core.Domain.Enums;

namespace TeamForge.Core.Domain.Entities;

public class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Nickname { get; set; } = string.Empty;

    public string? Description { get; set; }

    public UserRole Role { get; set; } = UserRole.User;

    public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Public;

    public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

    public ICollection<ActivityPeriod> ActivityPeriods { get; set; } = new List<ActivityPeriod>();

    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}