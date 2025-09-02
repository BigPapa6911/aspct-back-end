using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.DTOs.Communication;

namespace aspcts_backend.Services.Interfaces
{
    public interface ICommunicationService
    {
        Task<MessageResponse> SendMessageAsync(MessageRequest request, Guid senderId);
        Task<IEnumerable<MessageResponse>> GetMessagesByChildIdAsync(Guid childId, Guid userId, string userRole);
        Task<IEnumerable<MessageResponse>> GetConversationAsync(Guid otherUserId, Guid childId, Guid userId);
        Task<IEnumerable<MessageResponse>> GetUnreadMessagesAsync(Guid userId);
        Task<bool> MarkAsReadAsync(Guid messageId, Guid userId);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Dictionary<Guid, List<MessageResponse>>> GetConversationSummariesAsync(Guid userId, string userRole);
    }
}