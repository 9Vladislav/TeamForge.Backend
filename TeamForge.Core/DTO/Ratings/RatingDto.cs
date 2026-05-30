using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Comments;

namespace TeamForge.Core.DTO.Ratings;

public class RatingDto
{
    public int RatingId { get; set; }
    public int AuthorId { get; set; }
    public int ReceiverId { get; set; }
    public int InviteId { get; set; }
    public int Score { get; set; }
    public DateTime CreatedAt { get; set; }
    public CommentDto? Comment { get; set; }
}