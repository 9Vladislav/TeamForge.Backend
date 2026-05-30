using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Friends;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly IFriendService _friendService;

    public FriendsController(IFriendService friendService)
    {
        _friendService = friendService;
    }

    [HttpPost("requests")]
    public async Task<IActionResult> CreateRequest(CreateFriendRequestDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _friendService.CreateRequestAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("requests/incoming")]
    public async Task<IActionResult> GetIncomingRequests()
    {
        var userId = GetCurrentUserId();
        var result = await _friendService.GetIncomingRequestsAsync(userId);

        return Ok(result);
    }

    [HttpGet("requests/outgoing")]
    public async Task<IActionResult> GetOutgoingRequests()
    {
        var userId = GetCurrentUserId();
        var result = await _friendService.GetOutgoingRequestsAsync(userId);

        return Ok(result);
    }

    [HttpPut("requests/{requestId:int}")]
    public async Task<IActionResult> UpdateRequestStatus(
        int requestId,
        UpdateFriendRequestStatusDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _friendService.UpdateRequestStatusAsync(userId, requestId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetFriends()
    {
        var userId = GetCurrentUserId();
        var result = await _friendService.GetFriendsAsync(userId);

        return Ok(result);
    }

    [HttpDelete("{friendId:int}")]
    public async Task<IActionResult> DeleteFriend(int friendId)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _friendService.DeleteFriendAsync(userId, friendId);

            return Ok(new { message = "Користувача видалено зі списку друзів." });
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