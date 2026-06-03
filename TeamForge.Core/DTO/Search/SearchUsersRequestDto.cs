using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Search;

public class SearchUsersRequestDto
{
    public string? Nickname { get; set; }
    public int? GameId { get; set; }
    public string? SkillLevel { get; set; }
    public int? DayOfWeek { get; set; }
    public string? TimeFrom { get; set; }
    public string? TimeTo { get; set; }
    public double? MinRating { get; set; }
}