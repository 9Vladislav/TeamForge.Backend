using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.DTO.Games;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class GameService : IGameService
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GameDto>> GetAllAsync()
    {
        return await _context.Games
            .OrderBy(g => g.Name)
            .Select(g => MapToDto(g))
            .ToListAsync();
    }

    public async Task<GameDto> GetByIdAsync(int gameId)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game is null)
        {
            throw new Exception("Гру не знайдено.");
        }

        return MapToDto(game);
    }

    public async Task<GameDto> CreateAsync(CreateGameDto dto)
    {
        var name = dto.Name.Trim();

        var exists = await _context.Games
            .AnyAsync(g => g.Name.ToLower() == name.ToLower());

        if (exists)
        {
            throw new Exception("Гра з такою назвою вже існує.");
        }

        var game = new Game
        {
            Name = name,
            ImageUrl = dto.ImageUrl?.Trim()
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return MapToDto(game);
    }

    public async Task<GameDto> UpdateAsync(int gameId, UpdateGameDto dto)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game is null)
        {
            throw new Exception("Гру не знайдено.");
        }

        var name = dto.Name.Trim();

        var exists = await _context.Games
            .AnyAsync(g => g.Name.ToLower() == name.ToLower() && g.GameId != gameId);

        if (exists)
        {
            throw new Exception("Гра з такою назвою вже існує.");
        }

        game.Name = name;
        game.ImageUrl = dto.ImageUrl?.Trim();

        await _context.SaveChangesAsync();

        return MapToDto(game);
    }

    public async Task DeleteAsync(int gameId)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.GameId == gameId);

        if (game is null)
        {
            throw new Exception("Гру не знайдено.");
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
    }

    private static GameDto MapToDto(Game game)
    {
        return new GameDto
        {
            GameId = game.GameId,
            Name = game.Name,
            ImageUrl = game.ImageUrl
        };
    }
}