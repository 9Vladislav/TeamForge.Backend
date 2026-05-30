using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Comments;
using TeamForge.Core.DTO.Ratings;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class RatingService : IRatingService
{
    private readonly AppDbContext _context;

    public RatingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RatingDto> CreateRatingAsync(int authorId, CreateRatingDto dto)
    {
        if (dto.Score < 1 || dto.Score > 10)
        {
            throw new Exception("Оцінка має бути від 1 до 10.");
        }

        var invite = await _context.GameInvites
            .FirstOrDefaultAsync(i => i.InviteId == dto.InviteId);

        if (invite is null)
        {
            throw new Exception("Інвайт не знайдено.");
        }

        if (invite.Status != InviteStatus.Accepted)
        {
            throw new Exception("Оцінку можна залишити тільки після прийнятого інвайту.");
        }

        var isParticipant =
            invite.SenderId == authorId ||
            invite.ReceiverId == authorId;

        if (!isParticipant)
        {
            throw new Exception("Ви не є учасником цього інвайту.");
        }

        var receiverId = invite.SenderId == authorId
            ? invite.ReceiverId
            : invite.SenderId;

        var alreadyRated = await _context.Ratings
            .AnyAsync(r =>
                r.InviteId == dto.InviteId &&
                r.AuthorId == authorId);

        if (alreadyRated)
        {
            throw new Exception("Ви вже залишили оцінку за цим інвайтом.");
        }

        var rating = new Rating
        {
            AuthorId = authorId,
            ReceiverId = receiverId,
            InviteId = dto.InviteId,
            Score = dto.Score
        };

        _context.Ratings.Add(rating);

        await _context.SaveChangesAsync();

        if (!string.IsNullOrWhiteSpace(dto.CommentText))
        {
            var comment = new Comment
            {
                RatingId = rating.RatingId,
                AuthorId = authorId,
                ReceiverId = receiverId,
                Text = dto.CommentText.Trim()
            };

            _context.Comments.Add(comment);
        }

        var author = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == authorId);

        var hasComment = !string.IsNullOrWhiteSpace(dto.CommentText);

        var notificationMessage = hasComment
            ? $"Користувач {author?.Nickname ?? "невідомий"} оцінив вас на {dto.Score} балів та залишив коментар."
            : $"Користувач {author?.Nickname ?? "невідомий"} оцінив вас на {dto.Score} балів.";

        var notification = new Notification
        {
            UserId = receiverId,
            Type = NotificationType.Rating,
            MessageText = notificationMessage,
            IsRead = false
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        return await GetRatingDtoAsync(rating.RatingId);
    }

    public async Task<List<RatingDto>> GetUserRatingsAsync(int userId)
    {
        var ratings = await _context.Ratings
            .Include(r => r.Comments)
            .Where(r => r.ReceiverId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return ratings
            .Select(MapToDto)
            .ToList();
    }

    private async Task<RatingDto> GetRatingDtoAsync(int ratingId)
    {
        var rating = await _context.Ratings
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.RatingId == ratingId);

        if (rating is null)
        {
            throw new Exception("Оцінку не знайдено.");
        }

        return MapToDto(rating);
    }

    private static RatingDto MapToDto(Rating rating)
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