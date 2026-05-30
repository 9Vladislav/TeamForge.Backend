using System.Text.RegularExpressions;
using TeamForge.Core.DTO.Auth;

namespace TeamForge.Core.Validation;

public static class AuthValidator
{
    public static void ValidateRegister(RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new Exception("Електронна пошта є обов'язковою.");
        }

        if (!IsValidEmail(dto.Email))
        {
            throw new Exception("Некоректний формат електронної пошти.");
        }

        if (string.IsNullOrWhiteSpace(dto.Nickname))
        {
            throw new Exception("Нікнейм є обов'язковим.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new Exception("Пароль є обов'язковим.");
        }

        if (dto.Password.Length < 6)
        {
            throw new Exception("Пароль має містити щонайменше 6 символів.");
        }
    }

    public static void ValidateLogin(LoginDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new Exception("Електронна пошта є обов'язковою.");
        }

        if (!IsValidEmail(dto.Email))
        {
            throw new Exception("Некоректний формат електронної пошти.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new Exception("Пароль є обов'язковим.");
        }
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}