using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Profiles;

namespace TeamForge.Core.Services.Interfaces;

public interface IProfileService
{
    Task<ProfileDto> GetMyProfileAsync(int userId);
    Task<ProfileDto> GetProfileByUserIdAsync(int userId);
    Task<ProfileDto> UpdateMyProfileAsync(int userId, UpdateProfileDto dto);
    Task<UserGameDto> AddUserGameAsync(int userId, AddUserGameDto dto);
    Task<UserGameDto> UpdateUserGameAsync(int userId, int userGameId, UpdateUserGameDto dto);
    Task DeleteUserGameAsync(int userId, int userGameId);
    Task<ActivityPeriodDto> AddActivityPeriodAsync(int userId, AddActivityPeriodDto dto);
    Task<ActivityPeriodDto> UpdateActivityPeriodAsync(int userId, int activityPeriodId, UpdateActivityPeriodDto dto);
    Task DeleteActivityPeriodAsync(int userId, int activityPeriodId);
}