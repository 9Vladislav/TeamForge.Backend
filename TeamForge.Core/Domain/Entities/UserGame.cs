using TeamForge.Core.Domain.Enums;

namespace TeamForge.Core.Domain.Entities;

public class UserGame
{
    public int UserGameId { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public SkillLevel SkillLevel { get; set; }

    public string? PlaystyleDescription { get; set; }

    public User User { get; set; } = null!;

    public Game Game { get; set; } = null!;
}