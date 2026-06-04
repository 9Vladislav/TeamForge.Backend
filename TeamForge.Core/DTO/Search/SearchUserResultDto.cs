using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Search;

public class SearchUserResultDto
{
    public int UserId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Description { get; set; }
    public double AverageRating { get; set; }
    public List<SearchUserGameDto> Games { get; set; } = new();
    public List<SearchUserActivityPeriodDto> ActivityPeriods { get; set; } = new();
}