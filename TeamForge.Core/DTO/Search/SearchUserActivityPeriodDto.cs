using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Search;

public class SearchUserActivityPeriodDto
{
    public int ActivityPeriodId { get; set; }
    public int DayOfWeek { get; set; }
    public string TimeFrom { get; set; } = string.Empty;
    public string TimeTo { get; set; } = string.Empty;
}