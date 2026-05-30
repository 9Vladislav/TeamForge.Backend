using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Chats;

public class ChatDto
{
    public int ChatId { get; set; }
    public int OtherUserId { get; set; }
    public string OtherUserNickname { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? LastMessageText { get; set; }
    public DateTime? LastMessageAt { get; set; }
    public int UnreadMessagesCount { get; set; }
}