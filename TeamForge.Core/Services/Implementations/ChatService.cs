using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.DTO.Chats;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ChatDto>> GetMyChatsAsync(int userId)
    {
        var chats = await _context.Chats
            .Include(c => c.FirstUser)
            .Include(c => c.SecondUser)
            .Include(c => c.Messages)
            .Where(c => c.FirstUserId == userId || c.SecondUserId == userId)
            .ToListAsync();

        return chats
            .Select(c =>
            {
                var otherUser = c.FirstUserId == userId
                    ? c.SecondUser
                    : c.FirstUser;

                var lastMessage = c.Messages
                    .OrderByDescending(m => m.SentAt)
                    .FirstOrDefault();

                var unreadMessagesCount = c.Messages
                    .Count(m => m.SenderId != userId && !m.IsRead);

                return new ChatDto
                {
                    ChatId = c.ChatId,
                    OtherUserId = otherUser.UserId,
                    OtherUserNickname = otherUser.Nickname,
                    CreatedAt = c.CreatedAt,
                    LastMessageText = lastMessage?.Text,
                    LastMessageAt = lastMessage?.SentAt,
                    UnreadMessagesCount = unreadMessagesCount
                };
            })
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToList();
    }

    public async Task<List<MessageDto>> GetChatMessagesAsync(int userId, int chatId)
    {
        var chat = await _context.Chats
            .Include(c => c.Messages)
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c =>
                c.ChatId == chatId &&
                (c.FirstUserId == userId || c.SecondUserId == userId));

        if (chat is null)
        {
            throw new Exception("Чат не знайдено.");
        }

        return chat.Messages
            .OrderBy(m => m.SentAt)
            .Select(MapToMessageDto)
            .ToList();
    }

    public async Task<MessageDto> SendMessageAsync(int senderId, SendMessageDto dto)
    {
        if (senderId == dto.ReceiverId)
        {
            throw new Exception("Не можна надіслати повідомлення самому собі.");
        }

        if (string.IsNullOrWhiteSpace(dto.Text))
        {
            throw new Exception("Повідомлення не може бути порожнім.");
        }

        var sender = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == senderId);

        var receiver = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == dto.ReceiverId);

        if (sender is null || receiver is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        var firstUserId = Math.Min(senderId, dto.ReceiverId);
        var secondUserId = Math.Max(senderId, dto.ReceiverId);

        var chat = await _context.Chats
            .FirstOrDefaultAsync(c =>
                c.FirstUserId == firstUserId &&
                c.SecondUserId == secondUserId);

        if (chat is null)
        {
            chat = new Chat
            {
                FirstUserId = firstUserId,
                SecondUserId = secondUserId
            };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
        }

        var message = new Message
        {
            ChatId = chat.ChatId,
            SenderId = senderId,
            Text = dto.Text.Trim(),
            IsRead = false
        };

        _context.Messages.Add(message);

        await _context.SaveChangesAsync();

        message.Sender = sender;

        return MapToMessageDto(message);
    }

    public async Task MarkChatAsReadAsync(int userId, int chatId)
    {
        var chatExists = await _context.Chats
            .AnyAsync(c =>
                c.ChatId == chatId &&
                (c.FirstUserId == userId || c.SecondUserId == userId));

        if (!chatExists)
        {
            throw new Exception("Чат не знайдено.");
        }

        var unreadMessages = await _context.Messages
            .Where(m =>
                m.ChatId == chatId &&
                m.SenderId != userId &&
                !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    private static MessageDto MapToMessageDto(Message message)
    {
        return new MessageDto
        {
            MessageId = message.MessageId,
            ChatId = message.ChatId,
            SenderId = message.SenderId,
            SenderNickname = message.Sender.Nickname,
            Text = message.Text,
            SentAt = message.SentAt
        };
    }
}