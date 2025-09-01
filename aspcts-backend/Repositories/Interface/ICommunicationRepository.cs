using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Repositories.Interface
{
    public interface ICommunicationRepository : IGenericRepository<CommunicationMessage>
    {
        Task<IEnumerable<CommunicationMessage>> GetMessagesByChildIdAsync(Guid childId);
        Task<IEnumerable<CommunicationMessage>> GetConversationAsync(Guid userId1, Guid userId2, Guid childId);
        Task<IEnumerable<CommunicationMessage>> GetUnreadMessagesAsync(Guid userId);
        Task MarkAsReadAsync(Guid messageId);
        Task<int> GetUnreadCountAsync(Guid userId);
    }
}