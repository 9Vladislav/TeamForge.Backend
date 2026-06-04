using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Search;

public class SearchUserGameDto
{
    public int UserGameId { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string SkillLevel { get; set; } = string.Empty;
    public string? PlaystyleDescription { get; set; }
}