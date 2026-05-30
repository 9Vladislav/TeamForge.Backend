using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Profiles;

public class UpdateUserGameDto
{
    public string SkillLevel { get; set; } = string.Empty;
    public string? PlaystyleDescription { get; set; }
}