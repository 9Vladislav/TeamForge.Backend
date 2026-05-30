using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Chats;

public class MessageDto
{
    public int MessageId { get; set; }
    public int ChatId { get; set; }
    public int SenderId { get; set; }
    public string SenderNickname { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
}