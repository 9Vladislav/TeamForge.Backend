using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Admin;
using TeamForge.Core.DTO.Users;

namespace TeamForge.Core.Services.Interfaces;

public interface IAdminService
{
    Task<List<AdminUserDto>> GetUsersAsync();
    Task<AdminStatsDto> GetStatsAsync();
    Task<List<AdminCommentDto>> GetCommentsAsync();
    Task DeleteCommentAsync(int commentId);
    Task<AdminUserDto> UpdateUserAsync(int userId, UpdateAdminUserDto dto);
}