using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamForge.Core.DTO.Search;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpPost("users")]
    public async Task<IActionResult> SearchUsers(SearchUsersRequestDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var result = await _searchService.SearchUsersAsync(currentUserId, dto);

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