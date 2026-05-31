using System.Text.RegularExpressions;
using TeamForge.Core.DTO.Profiles;

namespace TeamForge.Core.Validation;

public static class ProfileValidator
{
    public static void ValidateUpdateProfile(UpdateProfileDto dto)
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

        if (string.IsNullOrWhiteSpace(dto.VisibilityStatus))
        {
            throw new Exception("Статус видимості профілю є обов'язковим.");
        }

        var wantsToChangePassword =
            !string.IsNullOrWhiteSpace(dto.CurrentPassword) ||
            !string.IsNullOrWhiteSpace(dto.NewPassword);

        if (wantsToChangePassword)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
            {
                throw new Exception("Введіть поточний пароль.");
            }

            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                throw new Exception("Введіть новий пароль.");
            }

            if (dto.NewPassword.Length < 6)
            {
                throw new Exception("Новий пароль має містити щонайменше 6 символів.");
            }
        }
    }

    public static void ValidateAddActivityPeriod(AddActivityPeriodDto dto)
    {
        ValidateActivityPeriod(dto.DayOfWeek, dto.TimeFrom, dto.TimeTo);
    }

    public static void ValidateUpdateActivityPeriod(UpdateActivityPeriodDto dto)
    {
        ValidateActivityPeriod(dto.DayOfWeek, dto.TimeFrom, dto.TimeTo);
    }

    private static void ValidateActivityPeriod(
        int dayOfWeek,
        string timeFromValue,
        string timeToValue)
    {
        if (dayOfWeek < 1 || dayOfWeek > 7)
        {
            throw new Exception("День тижня має бути від 1 до 7.");
        }

        if (!TimeOnly.TryParse(timeFromValue, out var timeFrom))
        {
            throw new Exception("Некоректний час початку активності.");
        }

        if (!TimeOnly.TryParse(timeToValue, out var timeTo))
        {
            throw new Exception("Некоректний час завершення активності.");
        }

        if (timeFrom >= timeTo)
        {
            throw new Exception("Час початку має бути меншим за час завершення.");
        }
    }

    private static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}