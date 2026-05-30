using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Admin;

public class AdminCommentDto
{
    public int CommentId { get; set; }
    public int AuthorId { get; set; }
    public string AuthorNickname { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverNickname { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}