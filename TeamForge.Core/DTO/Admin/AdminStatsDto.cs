using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Admin;

public class AdminStatsDto
{
    public int UsersCount { get; set; }
    public int PublicProfilesCount { get; set; }
    public int HiddenProfilesCount { get; set; }
    public List<PopularGameDto> PopularGames { get; set; } = new();
    public List<ActivityStatisticsDto> ActivityStatistics { get; set; } = new();
}

public class PopularGameDto
{
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public int PlayersCount { get; set; }
}

public class ActivityStatisticsDto
{
    public int DayOfWeek { get; set; }
    public int UsersCount { get; set; }
    public string? MostPopularTimeFrom { get; set; }
    public string? MostPopularTimeTo { get; set; }
}