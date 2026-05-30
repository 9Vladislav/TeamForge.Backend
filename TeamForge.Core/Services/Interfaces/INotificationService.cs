using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Notifications;

namespace TeamForge.Core.Services.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetMyNotificationsAsync(int userId);
    Task MarkAsReadAsync(int userId, int notificationId);
    Task MarkAllAsReadAsync(int userId);
}