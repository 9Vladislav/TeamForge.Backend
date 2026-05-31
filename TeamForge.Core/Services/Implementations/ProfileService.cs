using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Comments;
using TeamForge.Core.DTO.Profiles;
using TeamForge.Core.DTO.Ratings;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;
using TeamForge.Core.Validation;

namespace TeamForge.Core.Services.Implementations;

public class ProfileService : IProfileService
{
    private readonly AppDbContext _context;

    public ProfileService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ProfileDto> GetMyProfileAsync(int userId)
    {
        return await GetProfileByUserIdAsync(userId);
    }

    public async Task<ProfileDto> GetProfileByUserIdAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.UserGames)
                .ThenInclude(ug => ug.Game)
            .Include(u => u.ActivityPeriods)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        return await MapToProfileDtoAsync(user);
    }

    public async Task<ProfileDto> UpdateMyProfileAsync(int userId, UpdateProfileDto dto)
    {
        ProfileValidator.ValidateUpdateProfile(dto);

        var user = await _context.Users
            .Include(u => u.UserGames)
                .ThenInclude(ug => ug.Game)
            .Include(u => u.ActivityPeriods)
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        var email = dto.Email.Trim().ToLower();
        var nickname = dto.Nickname.Trim();

        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == email && u.UserId != userId);

        if (emailExists)
        {
            throw new Exception("Користувач з такою електронною поштою вже існує.");
        }

        var nicknameExists = await _context.Users
            .AnyAsync(u => u.Nickname == nickname && u.UserId != userId);

        if (nicknameExists)
        {
            throw new Exception("Користувач з таким нікнеймом вже існує.");
        }

        if (!Enum.TryParse<VisibilityStatus>(dto.VisibilityStatus, true, out var visibilityStatus))
        {
            throw new Exception("Некоректний статус видимості профілю.");
        }

        var wantsToChangePassword =
            !string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
            !string.IsNullOrWhiteSpace(dto.NewPassword);

        if (wantsToChangePassword)
        {
            var isCurrentPasswordValid = BCrypt.Net.BCrypt.Verify(
                dto.CurrentPassword,
                user.PasswordHash);

            if (!isCurrentPasswordValid)
            {
                throw new Exception("Поточний пароль введено неправильно.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        }

        user.Email = email;
        user.Nickname = nickname;
        user.Description = dto.Description?.Trim();
        user.VisibilityStatus = visibilityStatus;

        await _context.SaveChangesAsync();

        return await MapToProfileDtoAsync(user);
    }

    public async Task<UserGameDto> AddUserGameAsync(int userId, AddUserGameDto dto)
    {
        var game = await _context.Games
            .FirstOrDefaultAsync(g => g.GameId == dto.GameId);

        if (game is null)
        {
            throw new Exception("Гру не знайдено.");
        }

        var alreadyExists = await _context.UserGames
            .AnyAsync(ug => ug.UserId == userId && ug.GameId == dto.GameId);

        if (alreadyExists)
        {
            throw new Exception("Ця гра вже додана до профілю.");
        }

        if (!Enum.TryParse<SkillLevel>(dto.SkillLevel, true, out var skillLevel))
        {
            throw new Exception("Некоректний рівень навичок.");
        }

        var userGame = new UserGame
        {
            UserId = userId,
            GameId = dto.GameId,
            SkillLevel = skillLevel,
            PlaystyleDescription = dto.PlaystyleDescription?.Trim()
        };

        _context.UserGames.Add(userGame);
        await _context.SaveChangesAsync();

        userGame.Game = game;

        return MapToUserGameDto(userGame);
    }

    public async Task<UserGameDto> UpdateUserGameAsync(int userId, int userGameId, UpdateUserGameDto dto)
    {
        var userGame = await _context.UserGames
            .Include(ug => ug.Game)
            .FirstOrDefaultAsync(ug => ug.UserGameId == userGameId && ug.UserId == userId);

        if (userGame is null)
        {
            throw new Exception("Гру в профілі не знайдено.");
        }

        if (!Enum.TryParse<SkillLevel>(dto.SkillLevel, true, out var skillLevel))
        {
            throw new Exception("Некоректний рівень навичок.");
        }

        userGame.SkillLevel = skillLevel;
        userGame.PlaystyleDescription = dto.PlaystyleDescription?.Trim();

        await _context.SaveChangesAsync();

        return MapToUserGameDto(userGame);
    }

    public async Task DeleteUserGameAsync(int userId, int userGameId)
    {
        var userGame = await _context.UserGames
            .FirstOrDefaultAsync(ug => ug.UserGameId == userGameId && ug.UserId == userId);

        if (userGame is null)
        {
            throw new Exception("Гру в профілі не знайдено.");
        }

        _context.UserGames.Remove(userGame);
        await _context.SaveChangesAsync();
    }

    public async Task<ActivityPeriodDto> AddActivityPeriodAsync(int userId, AddActivityPeriodDto dto)
    {
        ProfileValidator.ValidateAddActivityPeriod(dto);

        TimeOnly.TryParse(dto.TimeFrom, out var timeFrom);
        TimeOnly.TryParse(dto.TimeTo, out var timeTo);

        var activityPeriod = new ActivityPeriod
        {
            UserId = userId,
            DayOfWeek = dto.DayOfWeek,
            TimeFrom = timeFrom,
            TimeTo = timeTo
        };

        _context.ActivityPeriods.Add(activityPeriod);
        await _context.SaveChangesAsync();

        return MapToActivityPeriodDto(activityPeriod);
    }

    public async Task<ActivityPeriodDto> UpdateActivityPeriodAsync(
        int userId,
        int activityPeriodId,
        UpdateActivityPeriodDto dto)
    {
        ProfileValidator.ValidateUpdateActivityPeriod(dto);

        TimeOnly.TryParse(dto.TimeFrom, out var timeFrom);
        TimeOnly.TryParse(dto.TimeTo, out var timeTo);

        var activityPeriod = await _context.ActivityPeriods
            .FirstOrDefaultAsync(ap =>
                ap.ActivityPeriodId == activityPeriodId &&
                ap.UserId == userId);

        if (activityPeriod is null)
        {
            throw new Exception("Період активності не знайдено.");
        }

        activityPeriod.DayOfWeek = dto.DayOfWeek;
        activityPeriod.TimeFrom = timeFrom;
        activityPeriod.TimeTo = timeTo;

        await _context.SaveChangesAsync();

        return MapToActivityPeriodDto(activityPeriod);
    }

    public async Task DeleteActivityPeriodAsync(int userId, int activityPeriodId)
    {
        var activityPeriod = await _context.ActivityPeriods
            .FirstOrDefaultAsync(ap => ap.ActivityPeriodId == activityPeriodId && ap.UserId == userId);

        if (activityPeriod is null)
        {
            throw new Exception("Період активності не знайдено.");
        }

        _context.ActivityPeriods.Remove(activityPeriod);
        await _context.SaveChangesAsync();
    }

    private async Task<ProfileDto> MapToProfileDtoAsync(User user)
    {
        var ratings = await _context.Ratings
            .Include(r => r.Comments)
            .Where(r => r.ReceiverId == user.UserId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return new ProfileDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Nickname = user.Nickname,
            Description = user.Description,
            VisibilityStatus = user.VisibilityStatus.ToString(),
            Games = user.UserGames.Select(MapToUserGameDto).ToList(),
            ActivityPeriods = user.ActivityPeriods.Select(MapToActivityPeriodDto).ToList(),
            AverageRating = ratings.Any()
                ? Math.Round(ratings.Average(r => r.Score), 1)
                : 0,

            Ratings = ratings
                .Where(r => r.Comments.Any())
                .Select(MapToRatingDto)
                .ToList()
        };
    }

    private static UserGameDto MapToUserGameDto(UserGame userGame)
    {
        return new UserGameDto
        {
            UserGameId = userGame.UserGameId,
            GameId = userGame.GameId,
            GameName = userGame.Game.Name,
            ImageUrl = userGame.Game.ImageUrl,
            SkillLevel = userGame.SkillLevel.ToString(),
            PlaystyleDescription = userGame.PlaystyleDescription
        };
    }

    private static ActivityPeriodDto MapToActivityPeriodDto(ActivityPeriod activityPeriod)
    {
        return new ActivityPeriodDto
        {
            ActivityPeriodId = activityPeriod.ActivityPeriodId,
            DayOfWeek = activityPeriod.DayOfWeek,
            TimeFrom = activityPeriod.TimeFrom.ToString("HH:mm"),
            TimeTo = activityPeriod.TimeTo.ToString("HH:mm")
        };
    }

    private static RatingDto MapToRatingDto(Rating rating)
    {
        var comment = rating.Comments.FirstOrDefault();

        return new RatingDto
        {
            RatingId = rating.RatingId,
            AuthorId = rating.AuthorId,
            ReceiverId = rating.ReceiverId,
            InviteId = rating.InviteId,
            Score = rating.Score,
            CreatedAt = rating.CreatedAt,
            Comment = comment is null
                ? null
                : new CommentDto
                {
                    CommentId = comment.CommentId,
                    AuthorId = comment.AuthorId,
                    ReceiverId = comment.ReceiverId,
                    Text = comment.Text,
                    CreatedAt = comment.CreatedAt
                }
        };
    }
}