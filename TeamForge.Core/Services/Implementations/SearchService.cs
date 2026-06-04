using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Search;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SearchUserResultDto>> SearchUsersAsync(
        int currentUserId,
        SearchUsersRequestDto dto)
    {
        SkillLevel? skillLevel = null;

        if (!string.IsNullOrWhiteSpace(dto.SkillLevel))
        {
            if (!Enum.TryParse<SkillLevel>(
                dto.SkillLevel,
                true,
                out var parsedSkillLevel))
            {
                throw new Exception("Некоректний рівень навичок.");
            }

            skillLevel = parsedSkillLevel;
        }

        TimeOnly? timeFrom = null;
        TimeOnly? timeTo = null;

        if (!string.IsNullOrWhiteSpace(dto.TimeFrom))
        {
            if (!TimeOnly.TryParse(dto.TimeFrom, out var parsedTimeFrom))
            {
                throw new Exception("Некоректний час початку активності.");
            }

            timeFrom = parsedTimeFrom;
        }

        if (!string.IsNullOrWhiteSpace(dto.TimeTo))
        {
            if (!TimeOnly.TryParse(dto.TimeTo, out var parsedTimeTo))
            {
                throw new Exception("Некоректний час завершення активності.");
            }

            timeTo = parsedTimeTo;
        }

        var query = _context.Users
            .Include(u => u.UserGames)
                .ThenInclude(ug => ug.Game)
            .Include(u => u.ActivityPeriods)
            .Where(u =>
                u.UserId != currentUserId &&
                u.VisibilityStatus == VisibilityStatus.Public);

        if (!string.IsNullOrWhiteSpace(dto.Nickname))
        {
            var nickname = dto.Nickname.Trim().ToLower();

            query = query.Where(u =>
                u.Nickname.ToLower().Contains(nickname));
        }

        if (dto.GameId.HasValue && dto.GameId.Value > 0)
        {
            query = query.Where(u =>
                u.UserGames.Any(ug => ug.GameId == dto.GameId.Value));
        }

        if (skillLevel.HasValue)
        {
            query = query.Where(u =>
                u.UserGames.Any(ug => ug.SkillLevel == skillLevel.Value));
        }

        if ((dto.DayOfWeek.HasValue && dto.DayOfWeek.Value > 0) ||
            timeFrom.HasValue ||
            timeTo.HasValue)
        {
            query = query.Where(u =>
                u.ActivityPeriods.Any(ap =>
                    (!dto.DayOfWeek.HasValue ||
                     dto.DayOfWeek.Value <= 0 ||
                     ap.DayOfWeek == dto.DayOfWeek.Value) &&

                    (!timeFrom.HasValue ||
                     ap.TimeFrom <= timeFrom.Value) &&

                    (!timeTo.HasValue ||
                     ap.TimeTo >= timeTo.Value)));
        }

        var users = await query
            .OrderBy(u => u.Nickname)
            .ToListAsync();

        var receiverIds = users
            .Select(u => u.UserId)
            .ToList();

        var ratingGroups = await _context.Ratings
            .Where(r => receiverIds.Contains(r.ReceiverId))
            .GroupBy(r => r.ReceiverId)
            .Select(g => new
            {
                ReceiverId = g.Key,
                AverageRating = g.Average(r => r.Score)
            })
            .ToListAsync();

        var ratingDictionary = ratingGroups
            .ToDictionary(
                x => x.ReceiverId,
                x => x.AverageRating);

        var result = users
            .Select(u =>
            {
                var averageRating =
                    ratingDictionary.TryGetValue(
                        u.UserId,
                        out var rating)
                    ? Math.Round(rating, 1)
                    : 0;

                return new SearchUserResultDto
                {
                    UserId = u.UserId,
                    Nickname = u.Nickname,
                    Description = u.Description,
                    AverageRating = averageRating,

                    Games = u.UserGames
                        .Select(ug => new SearchUserGameDto
                        {
                            UserGameId = ug.UserGameId,
                            GameId = ug.GameId,
                            GameName = ug.Game.Name,
                            ImageUrl = ug.Game.ImageUrl,
                            SkillLevel = ug.SkillLevel.ToString(),
                            PlaystyleDescription = ug.PlaystyleDescription
                        })
                        .ToList(),

                    ActivityPeriods = u.ActivityPeriods
                        .Select(ap => new SearchUserActivityPeriodDto
                        {
                            ActivityPeriodId = ap.ActivityPeriodId,
                            DayOfWeek = ap.DayOfWeek,
                            TimeFrom = ap.TimeFrom.ToString("HH:mm"),
                            TimeTo = ap.TimeTo.ToString("HH:mm")
                        })
                        .ToList()
                };
            })
            .Where(u =>
                !dto.MinRating.HasValue ||
                dto.MinRating.Value <= 0 ||
                u.AverageRating >= dto.MinRating.Value)
            .ToList();

        return result;
    }
}