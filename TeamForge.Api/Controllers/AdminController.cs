using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamForge.Core.DTO.Admin;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var result = await _adminService.GetUsersAsync();

        return Ok(result);
    }

    [HttpPut("users/{userId:int}")]
    public async Task<IActionResult> UpdateUser(int userId, UpdateAdminUserDto dto)
    {
        try
        {
            var result = await _adminService.UpdateUserAsync(userId, dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var result = await _adminService.GetStatsAsync();

        return Ok(result);
    }

    [HttpGet("comments")]
    public async Task<IActionResult> GetComments()
    {
        var result = await _adminService.GetCommentsAsync();

        return Ok(result);
    }

    [HttpDelete("comments/{commentId:int}")]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            await _adminService.DeleteCommentAsync(commentId);

            return Ok(new { message = "Коментар видалено." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}