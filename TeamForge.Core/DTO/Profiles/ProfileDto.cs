using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Ratings;

namespace TeamForge.Core.DTO.Profiles;

public class ProfileDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string VisibilityStatus { get; set; } = string.Empty;
    public List<UserGameDto> Games { get; set; } = new();
    public List<ActivityPeriodDto> ActivityPeriods { get; set; } = new();
    public double AverageRating { get; set; }
    public List<RatingDto> Ratings { get; set; } = new();
}