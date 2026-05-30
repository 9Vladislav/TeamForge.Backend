using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Chats;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatsController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatsController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyChats()
    {
        var userId = GetCurrentUserId();
        var result = await _chatService.GetMyChatsAsync(userId);

        return Ok(result);
    }

    [HttpGet("{chatId:int}/messages")]
    public async Task<IActionResult> GetChatMessages(int chatId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.GetChatMessagesAsync(userId, chatId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage(SendMessageDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _chatService.SendMessageAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{chatId:int}/read")]
    public async Task<IActionResult> MarkChatAsRead(int chatId)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _chatService.MarkChatAsReadAsync(userId, chatId);

            return Ok(new { message = "Повідомлення позначено як прочитані." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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