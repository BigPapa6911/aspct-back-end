using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using aspcts_backend.Data;
using aspcts_backend.Models.Entities;
using aspcts_backend.Repositories.Interface;

namespace aspcts_backend.Repositories
{
    public class CommunicationRepository : GenericRepository<CommunicationMessage>, ICommunicationRepository
    {
        public CommunicationRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<CommunicationMessage>> GetMessagesByChildIdAsync(Guid childId)
        {
            return await _context.CommunicationMessages
                .Include(cm => cm.Sender)
                .Include(cm => cm.Receiver)
                .Include(cm => cm.Child)
                .Where(cm => cm.ChildId == childId)
                .OrderByDescending(cm => cm.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunicationMessage>> GetConversationAsync(Guid userId1, Guid userId2, Guid childId)
        {
            return await _context.CommunicationMessages
                .Include(cm => cm.Sender)
                .Include(cm => cm.Receiver)
                .Include(cm => cm.Child)
                .Where(cm => cm.ChildId == childId &&
                             ((cm.SenderId == userId1 && cm.ReceiverId == userId2) ||
                              (cm.SenderId == userId2 && cm.ReceiverId == userId1)))
                .OrderBy(cm => cm.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<CommunicationMessage>> GetUnreadMessagesAsync(Guid userId)
        {
            return await _context.CommunicationMessages
                .Include(cm => cm.Sender)
                .Include(cm => cm.Child)
                .Where(cm => cm.ReceiverId == userId && !cm.IsRead)
                .OrderByDescending(cm => cm.Timestamp)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid messageId)
        {
            var message = await _context.CommunicationMessages.FindAsync(messageId);
            if (message != null)
            {
                message.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _context.CommunicationMessages
                .CountAsync(cm => cm.ReceiverId == userId && !cm.IsRead);
        }
    }
}