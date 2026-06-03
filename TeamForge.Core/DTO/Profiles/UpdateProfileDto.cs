using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Profiles;

public class UpdateProfileDto
{
    public string? Email { get; set; }
    public string? Nickname { get; set; }
    public string? Description { get; set; }
    public string? VisibilityStatus { get; set; }
    public string? CurrentPassword { get; set; }
    public string? NewPassword { get; set; }
}