using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Ratings;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RatingsController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingsController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateRating(CreateRatingDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _ratingService.CreateRatingAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetUserRatings(int userId)
    {
        var result = await _ratingService.GetUserRatingsAsync(userId);
        return Ok(result);
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