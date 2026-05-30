using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamForge.Core.DTO.Invites;

public class CreateGameInviteDto
{
    public int ReceiverId { get; set; }
    public int GameId { get; set; }
}