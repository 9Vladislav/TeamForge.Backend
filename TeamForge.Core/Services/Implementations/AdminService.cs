using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Admin;
using TeamForge.Core.DTO.Users;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class AdminService : IAdminService
{
    private readonly AppDbContext _context;

    public AdminService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<AdminUserDto>> GetUsersAsync()
    {
        return await _context.Users
            .OrderBy(u => u.UserId)
            .Select(u => new AdminUserDto
            {
                UserId = u.UserId,
                Email = u.Email,
                Nickname = u.Nickname,
                Description = u.Description,
                Role = u.Role.ToString(),
                VisibilityStatus = u.VisibilityStatus.ToString()
            })
            .ToListAsync();
    }

    public async Task<AdminUserDto> UpdateUserAsync(int userId, UpdateAdminUserDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        var nickname = dto.Nickname.Trim();

        var nicknameExists = await _context.Users
            .AnyAsync(u => u.Nickname == nickname && u.UserId != userId);

        if (nicknameExists)
        {
            throw new Exception("Користувач з таким нікнеймом вже існує.");
        }

        if (!Enum.TryParse<UserRole>(dto.Role, true, out var role))
        {
            throw new Exception("Некоректна роль користувача.");
        }

        if (!Enum.TryParse<VisibilityStatus>(dto.VisibilityStatus, true, out var visibilityStatus))
        {
            throw new Exception("Некоректний статус видимості профілю.");
        }

        user.Nickname = nickname;
        user.Description = dto.Description?.Trim();
        user.Role = role;
        user.VisibilityStatus = visibilityStatus;

        await _context.SaveChangesAsync();

        return new AdminUserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Nickname = user.Nickname,
            Description = user.Description,
            Role = user.Role.ToString(),
            VisibilityStatus = user.VisibilityStatus.ToString()
        };
    }

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var popularGames = await _context.UserGames
            .Include(ug => ug.Game)
            .GroupBy(ug => new
            {
                ug.GameId,
                ug.Game.Name
            })
            .Select(g => new PopularGameDto
            {
                GameId = g.Key.GameId,
                GameName = g.Key.Name,
                PlayersCount = g.Count()
            })
            .OrderByDescending(g => g.PlayersCount)
            .ToListAsync();

        var activityPeriods = await _context.ActivityPeriods
            .ToListAsync();

        var activityStatistics = activityPeriods
            .GroupBy(ap => ap.DayOfWeek)
            .Select(dayGroup =>
            {
                var mostPopularTime = dayGroup
                    .GroupBy(ap => new
                    {
                        ap.TimeFrom,
                        ap.TimeTo
                    })
                    .OrderByDescending(timeGroup => timeGroup.Count())
                    .FirstOrDefault();

                return new ActivityStatisticsDto
                {
                    DayOfWeek = dayGroup.Key,
                    UsersCount = dayGroup
                        .Select(ap => ap.UserId)
                        .Distinct()
                        .Count(),
                    MostPopularTimeFrom = mostPopularTime?.Key.TimeFrom.ToString("HH:mm"),
                    MostPopularTimeTo = mostPopularTime?.Key.TimeTo.ToString("HH:mm")
                };
            })
            .OrderBy(a => a.DayOfWeek)
            .ToList();

        return new AdminStatsDto
        {
            UsersCount = await _context.Users.CountAsync(),
            PublicProfilesCount = await _context.Users
                .CountAsync(u => u.VisibilityStatus == VisibilityStatus.Public),
            HiddenProfilesCount = await _context.Users
                .CountAsync(u => u.VisibilityStatus == VisibilityStatus.Hidden),
            PopularGames = popularGames,
            ActivityStatistics = activityStatistics
        };
    }

    public async Task<List<AdminCommentDto>> GetCommentsAsync()
    {
        return await _context.Comments
            .Include(c => c.Author)
            .Include(c => c.Receiver)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new AdminCommentDto
            {
                CommentId = c.CommentId,
                AuthorId = c.AuthorId,
                AuthorNickname = c.Author.Nickname,
                ReceiverId = c.ReceiverId,
                ReceiverNickname = c.Receiver.Nickname,
                Text = c.Text,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();
    }

    public async Task DeleteCommentAsync(int commentId)
    {
        var comment = await _context.Comments
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment is null)
        {
            throw new Exception("Коментар не знайдено.");
        }

        _context.Comments.Remove(comment);

        await _context.SaveChangesAsync();
    }
}