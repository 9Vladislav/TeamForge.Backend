using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamForge.Core.DTO.Chats;

namespace TeamForge.Core.Services.Interfaces;

public interface IChatService
{
    Task<List<ChatDto>> GetMyChatsAsync(int userId);
    Task<List<MessageDto>> GetChatMessagesAsync(int userId, int chatId);
    Task<MessageDto> SendMessageAsync(int senderId, SendMessageDto dto);
    Task MarkChatAsReadAsync(int userId, int chatId);
}