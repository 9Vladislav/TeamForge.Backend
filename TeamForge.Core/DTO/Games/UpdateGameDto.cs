using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Games;

public class UpdateGameDto
{
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}