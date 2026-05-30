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

        if (dto.GameId.HasValue)
        {
            query = query.Where(u =>
                u.UserGames.Any(ug => ug.GameId == dto.GameId.Value));
        }

        if (skillLevel.HasValue)
        {
            query = query.Where(u =>
                u.UserGames.Any(ug => ug.SkillLevel == skillLevel.Value));
        }

        if (dto.DayOfWeek.HasValue || timeFrom.HasValue || timeTo.HasValue)
        {
            query = query.Where(u =>
                u.ActivityPeriods.Any(ap =>
                    (!dto.DayOfWeek.HasValue ||
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
                    ? Math.Round(rating, 2)
                    : 0;

                return new SearchUserResultDto
                {
                    UserId = u.UserId,
                    Nickname = u.Nickname,
                    Description = u.Description,
                    AverageRating = averageRating,

                    Games = u.UserGames
                        .Select(ug => ug.Game.Name)
                        .Distinct()
                        .ToList(),

                    SkillLevels = u.UserGames
                        .Select(ug => ug.SkillLevel.ToString())
                        .Distinct()
                        .ToList()
                };
            })
            .Where(u =>
                !dto.MinRating.HasValue ||
                u.AverageRating >= dto.MinRating.Value)
            .ToList();

        return result;
    }
}