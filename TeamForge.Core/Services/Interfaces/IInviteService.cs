using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Invites;

namespace TeamForge.Core.Services.Interfaces;

public interface IInviteService
{
    Task<GameInviteDto> CreateInviteAsync(int senderId, CreateGameInviteDto dto);
    Task<List<GameInviteDto>> GetIncomingInvitesAsync(int userId);
    Task<List<GameInviteDto>> GetOutgoingInvitesAsync(int userId);
    Task<List<GameInviteDto>> GetAcceptedInvitesAsync(int userId);
    Task<List<GameInviteDto>> GetHistoryInvitesAsync(int userId);
    Task<GameInviteDto> UpdateInviteStatusAsync(int userId, int inviteId, UpdateInviteStatusDto dto);
}