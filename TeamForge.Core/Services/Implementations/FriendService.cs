using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Friends;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class FriendService : IFriendService
{
    private readonly AppDbContext _context;

    public FriendService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FriendRequestDto> CreateRequestAsync(int senderId, CreateFriendRequestDto dto)
    {
        if (senderId == dto.ReceiverId)
        {
            throw new Exception("Не можна надіслати запит у друзі самому собі.");
        }

        var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserId == senderId);
        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.ReceiverId);

        if (receiver is null || sender is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        var alreadyFriends = await _context.Friends
            .AnyAsync(f => f.UserId == senderId && f.FriendUserId == dto.ReceiverId);

        if (alreadyFriends)
        {
            throw new Exception("Користувач вже є у списку друзів.");
        }

        var pendingRequestExists = await _context.FriendRequests
            .AnyAsync(fr =>
                fr.Status == FriendRequestStatus.Pending &&
                ((fr.SenderId == senderId && fr.ReceiverId == dto.ReceiverId) ||
                 (fr.SenderId == dto.ReceiverId && fr.ReceiverId == senderId)));

        if (pendingRequestExists)
        {
            throw new Exception("Запит у друзі вже існує.");
        }

        var request = new FriendRequest
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Status = FriendRequestStatus.Pending
        };

        _context.FriendRequests.Add(request);

        _context.Notifications.Add(new Notification
        {
            UserId = dto.ReceiverId,
            Type = NotificationType.FriendRequest,
            MessageText = $"Користувач {sender.Nickname} надіслав вам запит у друзі.",
            IsRead = false
        });

        await _context.SaveChangesAsync();

        return await GetRequestDtoAsync(request.FriendRequestId);
    }

    public async Task<List<FriendRequestDto>> GetIncomingRequestsAsync(int userId)
    {
        return await _context.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
            .OrderByDescending(fr => fr.CreatedAt)
            .Select(fr => MapToRequestDto(fr))
            .ToListAsync();
    }

    public async Task<List<FriendRequestDto>> GetOutgoingRequestsAsync(int userId)
    {
        return await _context.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .Where(fr => fr.SenderId == userId && fr.Status == FriendRequestStatus.Pending)
            .OrderByDescending(fr => fr.CreatedAt)
            .Select(fr => MapToRequestDto(fr))
            .ToListAsync();
    }

    public async Task<FriendRequestDto> UpdateRequestStatusAsync(
        int userId,
        int requestId,
        UpdateFriendRequestStatusDto dto)
    {
        var request = await _context.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId);

        if (request is null)
        {
            throw new Exception("Запит у друзі не знайдено.");
        }

        if (request.Status != FriendRequestStatus.Pending)
        {
            throw new Exception("Цей запит вже оброблено.");
        }

        if (!Enum.TryParse<FriendRequestStatus>(dto.Status, true, out var status))
        {
            throw new Exception("Некоректний статус запиту.");
        }

        if (status == FriendRequestStatus.Cancelled)
        {
            if (request.SenderId != userId)
            {
                throw new Exception("Скасувати можна тільки власний запит у друзі.");
            }

            request.Status = FriendRequestStatus.Cancelled;

            await _context.SaveChangesAsync();

            return MapToRequestDto(request);
        }

        if (request.ReceiverId != userId)
        {
            throw new Exception("Обробити можна тільки вхідний запит у друзі.");
        }

        if (status != FriendRequestStatus.Accepted &&
            status != FriendRequestStatus.Declined)
        {
            throw new Exception("Запит можна тільки прийняти, відхилити або скасувати.");
        }

        request.Status = status;

        if (status == FriendRequestStatus.Accepted)
        {
            _context.Friends.Add(new Friend
            {
                UserId = request.SenderId,
                FriendUserId = request.ReceiverId
            });

            _context.Friends.Add(new Friend
            {
                UserId = request.ReceiverId,
                FriendUserId = request.SenderId
            });

            _context.Notifications.Add(new Notification
            {
                UserId = request.SenderId,
                Type = NotificationType.FriendRequest,
                MessageText = $"Користувач {request.Receiver.Nickname} прийняв ваш запит у друзі.",
                IsRead = false
            });
        }
        else
        {
            _context.Notifications.Add(new Notification
            {
                UserId = request.SenderId,
                Type = NotificationType.FriendRequest,
                MessageText = $"Користувач {request.Receiver.Nickname} відхилив ваш запит у друзі.",
                IsRead = false
            });
        }

        await _context.SaveChangesAsync();

        return MapToRequestDto(request);
    }

    public async Task<List<FriendDto>> GetFriendsAsync(int userId)
    {
        return await _context.Friends
            .Include(f => f.FriendUser)
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.FriendUser.Nickname)
            .Select(f => new FriendDto
            {
                FriendId = f.FriendId,
                UserId = f.FriendUser.UserId,
                Nickname = f.FriendUser.Nickname,
                Description = f.FriendUser.Description
            })
            .ToListAsync();
    }

    public async Task DeleteFriendAsync(int userId, int friendId)
    {
        var friend = await _context.Friends
            .FirstOrDefaultAsync(f => f.FriendId == friendId && f.UserId == userId);

        if (friend is null)
        {
            throw new Exception("Користувача немає у списку друзів.");
        }

        var reverseFriend = await _context.Friends
            .FirstOrDefaultAsync(f =>
                f.UserId == friend.FriendUserId &&
                f.FriendUserId == userId);

        _context.Friends.Remove(friend);

        if (reverseFriend is not null)
        {
            _context.Friends.Remove(reverseFriend);
        }

        await _context.SaveChangesAsync();
    }

    private async Task<FriendRequestDto> GetRequestDtoAsync(int requestId)
    {
        var request = await _context.FriendRequests
            .Include(fr => fr.Sender)
            .Include(fr => fr.Receiver)
            .FirstOrDefaultAsync(fr => fr.FriendRequestId == requestId);

        if (request is null)
        {
            throw new Exception("Запит у друзі не знайдено.");
        }

        return MapToRequestDto(request);
    }

    private static FriendRequestDto MapToRequestDto(FriendRequest request)
    {
        return new FriendRequestDto
        {
            FriendRequestId = request.FriendRequestId,
            SenderId = request.SenderId,
            SenderNickname = request.Sender.Nickname,
            ReceiverId = request.ReceiverId,
            ReceiverNickname = request.Receiver.Nickname,
            Status = request.Status.ToString(),
            CreatedAt = request.CreatedAt
        };
    }
}