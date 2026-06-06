using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Invites;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class InviteService : IInviteService
{
    private readonly AppDbContext _context;

    public InviteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GameInviteDto> CreateInviteAsync(int senderId, CreateGameInviteDto dto)
    {
        if (senderId == dto.ReceiverId)
        {
            throw new Exception("Не можна надіслати інвайт самому собі.");
        }

        var sender = await _context.Users.FirstOrDefaultAsync(u => u.UserId == senderId);
        var receiver = await _context.Users.FirstOrDefaultAsync(u => u.UserId == dto.ReceiverId);

        if (sender is null || receiver is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        var game = await _context.Games.FirstOrDefaultAsync(g => g.GameId == dto.GameId);

        if (game is null)
        {
            throw new Exception("Гру не знайдено.");
        }

        var pendingInviteExists = await _context.GameInvites
            .AnyAsync(i =>
                i.SenderId == senderId &&
                i.ReceiverId == dto.ReceiverId &&
                i.GameId == dto.GameId &&
                i.Status == InviteStatus.Pending);

        if (pendingInviteExists)
        {
            throw new Exception("Активний інвайт для цієї гри вже існує.");
        }

        var invite = new GameInvite
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            GameId = dto.GameId,
            Status = InviteStatus.Pending
        };

        _context.GameInvites.Add(invite);

        _context.Notifications.Add(new Notification
        {
            UserId = dto.ReceiverId,
            Type = NotificationType.Invite,
            MessageText = $"Користувач {sender.Nickname} надіслав вам інвайт у гру {game.Name}.",
            IsRead = false
        });

        await _context.SaveChangesAsync();

        return await GetInviteDtoAsync(invite.InviteId, senderId);
    }

    public async Task<List<GameInviteDto>> GetIncomingInvitesAsync(int userId)
    {
        var invites = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .Where(i => i.ReceiverId == userId && i.Status == InviteStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return await MapToDtoListAsync(invites, userId);
    }

    public async Task<List<GameInviteDto>> GetOutgoingInvitesAsync(int userId)
    {
        var invites = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .Where(i => i.SenderId == userId && i.Status == InviteStatus.Pending)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return await MapToDtoListAsync(invites, userId);
    }

    public async Task<List<GameInviteDto>> GetAcceptedInvitesAsync(int userId)
    {
        var invites = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .Where(i =>
                (i.SenderId == userId || i.ReceiverId == userId) &&
                i.Status == InviteStatus.Accepted)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return await MapToDtoListAsync(invites, userId);
    }

    public async Task<List<GameInviteDto>> GetHistoryInvitesAsync(int userId)
    {
        var invites = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .Where(i =>
                (
                    (i.SenderId == userId || i.ReceiverId == userId) &&
                    i.Status == InviteStatus.Accepted
                )
                ||
                (
                    i.ReceiverId == userId &&
                    i.Status == InviteStatus.Cancelled
                ))
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return await MapToDtoListAsync(invites, userId);
    }

    public async Task<GameInviteDto> UpdateInviteStatusAsync(
        int userId,
        int inviteId,
        UpdateInviteStatusDto dto)
    {
        var invite = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .FirstOrDefaultAsync(i => i.InviteId == inviteId);

        if (invite is null)
        {
            throw new Exception("Інвайт не знайдено.");
        }

        if (invite.Status != InviteStatus.Pending)
        {
            throw new Exception("Цей інвайт вже оброблено.");
        }

        if (!Enum.TryParse<InviteStatus>(dto.Status, true, out var status))
        {
            throw new Exception("Некоректний статус інвайту.");
        }

        if (status == InviteStatus.Accepted || status == InviteStatus.Declined)
        {
            if (invite.ReceiverId != userId)
            {
                throw new Exception("Тільки отримувач може прийняти або відхилити інвайт.");
            }
        }
        else if (status == InviteStatus.Cancelled)
        {
            if (invite.SenderId != userId)
            {
                throw new Exception("Тільки відправник може скасувати інвайт.");
            }
        }
        else
        {
            throw new Exception("Інвайт можна тільки прийняти, відхилити або скасувати.");
        }

        invite.Status = status;

        if (status == InviteStatus.Accepted)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = invite.SenderId,
                Type = NotificationType.Invite,
                MessageText = $"Користувач {invite.Receiver.Nickname} прийняв ваш інвайт у гру {invite.Game.Name}.",
                IsRead = false
            });
        }
        else if (status == InviteStatus.Declined)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = invite.SenderId,
                Type = NotificationType.Invite,
                MessageText = $"Користувач {invite.Receiver.Nickname} відхилив ваш інвайт у гру {invite.Game.Name}.",
                IsRead = false
            });
        }

        await _context.SaveChangesAsync();

        return await GetInviteDtoAsync(invite.InviteId, userId);
    }

    private async Task<GameInviteDto> GetInviteDtoAsync(int inviteId, int currentUserId)
    {
        var invite = await _context.GameInvites
            .Include(i => i.Sender)
            .Include(i => i.Receiver)
            .Include(i => i.Game)
            .FirstOrDefaultAsync(i => i.InviteId == inviteId);

        if (invite is null)
        {
            throw new Exception("Інвайт не знайдено.");
        }

        return await MapToDtoAsync(invite, currentUserId);
    }

    private async Task<List<GameInviteDto>> MapToDtoListAsync(
        List<GameInvite> invites,
        int currentUserId)
    {
        var result = new List<GameInviteDto>();

        foreach (var invite in invites)
        {
            result.Add(await MapToDtoAsync(invite, currentUserId));
        }

        return result;
    }

    private async Task<GameInviteDto> MapToDtoAsync(GameInvite invite, int currentUserId)
    {
        var ratedUserId = invite.SenderId == currentUserId
            ? invite.ReceiverId
            : invite.SenderId;

        var isRatedByCurrentUser = await _context.Ratings
            .AnyAsync(r =>
                r.InviteId == invite.InviteId &&
                r.AuthorId == currentUserId);

        return new GameInviteDto
        {
            InviteId = invite.InviteId,
            SenderId = invite.SenderId,
            SenderNickname = invite.Sender.Nickname,
            ReceiverId = invite.ReceiverId,
            ReceiverNickname = invite.Receiver.Nickname,
            GameId = invite.GameId,
            GameName = invite.Game.Name,
            Status = invite.Status.ToString(),
            CreatedAt = invite.CreatedAt,
            IsRatedByCurrentUser = isRatedByCurrentUser,
            RatedUserId = ratedUserId
        };
    }
}