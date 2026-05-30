using Microsoft.EntityFrameworkCore;
using TeamForge.Core.Domain.Enums;
using TeamForge.Core.DTO.Users;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;

namespace TeamForge.Core.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> GetCurrentUserAsync(int userId)
    {
        return await GetByIdAsync(userId);
    }

    public async Task<UserDto> GetByIdAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);

        if (user is null)
        {
            throw new Exception("Користувача не знайдено.");
        }

        return MapToDto(user);
    }

    private static UserDto MapToDto(TeamForge.Core.Domain.Entities.User user)
    {
        return new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Nickname = user.Nickname,
            Description = user.Description,
            Role = user.Role.ToString(),
            VisibilityStatus = user.VisibilityStatus.ToString()
        };
    }
}