using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Games;

namespace TeamForge.Core.Services.Interfaces;

public interface IGameService
{
    Task<List<GameDto>> GetAllAsync();
    Task<GameDto> GetByIdAsync(int gameId);
    Task<GameDto> CreateAsync(CreateGameDto dto);
    Task<GameDto> UpdateAsync(int gameId, UpdateGameDto dto);
    Task DeleteAsync(int gameId);
}