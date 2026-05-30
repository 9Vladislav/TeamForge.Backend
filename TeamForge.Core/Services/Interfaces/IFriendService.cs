using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Friends;

namespace TeamForge.Core.Services.Interfaces;

public interface IFriendService
{
    Task<FriendRequestDto> CreateRequestAsync(int senderId, CreateFriendRequestDto dto);
    Task<List<FriendRequestDto>> GetIncomingRequestsAsync(int userId);
    Task<List<FriendRequestDto>> GetOutgoingRequestsAsync(int userId);
    Task<FriendRequestDto> UpdateRequestStatusAsync(int userId, int requestId, UpdateFriendRequestStatusDto dto);
    Task<List<FriendDto>> GetFriendsAsync(int userId);
    Task DeleteFriendAsync(int userId, int friendId);
}