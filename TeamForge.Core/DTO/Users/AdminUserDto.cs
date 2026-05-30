using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Users;

public class AdminUserDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Role { get; set; } = string.Empty;
    public string VisibilityStatus { get; set; } = string.Empty;
}