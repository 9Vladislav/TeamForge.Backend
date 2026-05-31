using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Profiles;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfilesController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfilesController(IProfileService profileService)
    {
        _profileService = profileService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.GetMyProfileAsync(userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetProfileByUserId(int userId)
    {
        try
        {
            var result = await _profileService.GetProfileByUserIdAsync(userId);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile(UpdateProfileDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.UpdateMyProfileAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("games")]
    public async Task<IActionResult> AddUserGame(AddUserGameDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.AddUserGameAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("games/{userGameId:int}")]
    public async Task<IActionResult> UpdateUserGame(int userGameId, UpdateUserGameDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.UpdateUserGameAsync(userId, userGameId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("games/{userGameId:int}")]
    public async Task<IActionResult> DeleteUserGame(int userGameId)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _profileService.DeleteUserGameAsync(userId, userGameId);

            return Ok(new { message = "Гру видалено з профілю." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("activity-periods")]
    public async Task<IActionResult> AddActivityPeriod(AddActivityPeriodDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.AddActivityPeriodAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("activity-periods/{activityPeriodId:int}")]
    public async Task<IActionResult> UpdateActivityPeriod(
        int activityPeriodId,
        UpdateActivityPeriodDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();

            var result = await _profileService.UpdateActivityPeriodAsync(
                userId,
                activityPeriodId,
                dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("activity-periods/{activityPeriodId:int}")]
    public async Task<IActionResult> DeleteActivityPeriod(int activityPeriodId)
    {
        try
        {
            var userId = GetCurrentUserId();

            await _profileService.DeleteActivityPeriodAsync(userId, activityPeriodId);

            return Ok(new { message = "Період активності видалено." });
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