using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Invites;

public class GameInviteDto
{
    public int InviteId { get; set; }
    public int SenderId { get; set; }
    public string SenderNickname { get; set; } = string.Empty;
    public int ReceiverId { get; set; }
    public string ReceiverNickname { get; set; } = string.Empty;
    public int GameId { get; set; }
    public string GameName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRatedByCurrentUser { get; set; }
    public int? RatedUserId { get; set; }
}