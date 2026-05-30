using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Invites;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InvitesController : ControllerBase
{
    private readonly IInviteService _inviteService;

    public InvitesController(IInviteService inviteService)
    {
        _inviteService = inviteService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateInvite(CreateGameInviteDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _inviteService.CreateInviteAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("incoming")]
    public async Task<IActionResult> GetIncomingInvites()
    {
        var userId = GetCurrentUserId();
        var result = await _inviteService.GetIncomingInvitesAsync(userId);

        return Ok(result);
    }

    [HttpGet("outgoing")]
    public async Task<IActionResult> GetOutgoingInvites()
    {
        var userId = GetCurrentUserId();
        var result = await _inviteService.GetOutgoingInvitesAsync(userId);

        return Ok(result);
    }

    [HttpGet("accepted")]
    public async Task<IActionResult> GetAcceptedInvites()
    {
        var userId = GetCurrentUserId();
        var result = await _inviteService.GetAcceptedInvitesAsync(userId);

        return Ok(result);
    }

    [HttpPut("{inviteId:int}/status")]
    public async Task<IActionResult> UpdateInviteStatus(
        int inviteId,
        UpdateInviteStatusDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _inviteService.UpdateInviteStatusAsync(userId, inviteId, dto);

            return Ok(result);
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