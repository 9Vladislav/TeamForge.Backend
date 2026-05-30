using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Chats;

public class SendMessageDto
{
    public int ReceiverId { get; set; }
    public string Text { get; set; } = string.Empty;
}