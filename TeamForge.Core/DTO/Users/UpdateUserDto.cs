using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Users;

public class UpdateUserDto
{
    public string Nickname { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VisibilityStatus { get; set; } = string.Empty;
}