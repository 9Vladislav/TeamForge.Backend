using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Comments;

public class CommentDto
{
    public int CommentId { get; set; }
    public int AuthorId { get; set; }
    public int ReceiverId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}