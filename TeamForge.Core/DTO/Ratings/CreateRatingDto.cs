using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Ratings;

public class CreateRatingDto
{
    public int InviteId { get; set; }
    public int Score { get; set; }
    public string? CommentText { get; set; }
}