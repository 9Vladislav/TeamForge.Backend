using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamForge.Core.DTO.Games;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _gameService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{gameId:int}")]
    public async Task<IActionResult> GetById(int gameId)
    {
        try
        {
            var result = await _gameService.GetByIdAsync(gameId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(CreateGameDto dto)
    {
        try
        {
            var result = await _gameService.CreateAsync(dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{gameId:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int gameId, UpdateGameDto dto)
    {
        try
        {
            var result = await _gameService.UpdateAsync(gameId, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{gameId:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int gameId)
    {
        try
        {
            await _gameService.DeleteAsync(gameId);
            return Ok(new { message = "Гру видалено." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}