using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TeamForge.Core.Domain.Entities;
using TeamForge.Core.DTO.Auth;
using TeamForge.Core.Persistence;
using TeamForge.Core.Services.Interfaces;
using TeamForge.Core.Validation;

namespace TeamForge.Core.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtOptions _jwtOptions;

    public AuthService(AppDbContext context, IOptions<JwtOptions> jwtOptions)
    {
        _context = context;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        AuthValidator.ValidateRegister(dto);

        var email = dto.Email.Trim().ToLower();
        var nickname = dto.Nickname.Trim();

        var emailExists = await _context.Users.AnyAsync(u => u.Email == email);

        if (emailExists)
        {
            throw new Exception("Користувач з такою електронною поштою вже існує.");
        }

        var nicknameExists = await _context.Users.AnyAsync(u => u.Nickname == nickname);

        if (nicknameExists)
        {
            throw new Exception("Користувач з таким нікнеймом вже існує.");
        }

        var user = new User
        {
            Email = email,
            Nickname = nickname,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        AuthValidator.ValidateLogin(dto);

        var email = dto.Email.Trim().ToLower();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            throw new Exception("Невірна електронна пошта або пароль.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new Exception("Невірна електронна пошта або пароль.");
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            Nickname = user.Nickname,
            Role = user.Role.ToString(),
            Token = GenerateJwtToken(user)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Nickname),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}