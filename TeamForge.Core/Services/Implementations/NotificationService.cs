using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.DTO.Notifications;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _context;

    public NotificationService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> GetMyNotificationsAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => MapToDto(n))
            .ToListAsync();
    }

    public async Task MarkAsReadAsync(int userId, int notificationId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n =>
                n.NotificationId == notificationId &&
                n.UserId == userId);

        if (notification is null)
        {
            throw new Exception("Сповіщення не знайдено.");
        }

        notification.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();
    }

    private static NotificationDto MapToDto(Notification notification)
    {
        return new NotificationDto
        {
            NotificationId = notification.NotificationId,
            Type = notification.Type.ToString(),
            MessageText = notification.MessageText,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };
    }
}