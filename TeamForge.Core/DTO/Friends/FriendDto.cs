using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Friends;

public class FriendDto
{
    public int FriendId { get; set; }
    public int UserId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string? Description { get; set; }
}