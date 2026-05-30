namespace TeamForge.Core.Domain.Entities;

public class Game
{
    public int GameId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public ICollection<UserGame> UserGames { get; set; } = new List<UserGame>();

    public ICollection<GameInvite> GameInvites { get; set; } = new List<GameInvite>();
}