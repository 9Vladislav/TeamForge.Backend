using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Users;

namespace TeamForge.Core.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> GetCurrentUserAsync(int userId);
    Task<UserDto> GetByIdAsync(int userId);
}