using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications()
    {
        var userId = GetCurrentUserId();
        var result = await _notificationService.GetMyNotificationsAsync(userId);

        return Ok(result);
    }

    [HttpPut("{notificationId:int}/read")]
    public async Task<IActionResult> MarkAsRead(int notificationId)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _notificationService.MarkAsReadAsync(userId, notificationId);

            return Ok(new { message = "Сповіщення позначено як прочитане." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userId = GetCurrentUserId();

        await _notificationService.MarkAllAsReadAsync(userId);

        return Ok(new { message = "Усі сповіщення позначено як прочитані." });
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdValue))
        {
            throw new Exception("Не вдалося визначити користувача.");
        }

        return int.Parse(userIdValue);
    }
}