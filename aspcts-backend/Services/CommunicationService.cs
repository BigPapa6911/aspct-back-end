using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using aspcts_backend.Services.Interfaces;
using aspcts_backend.Repositories.Interface;
using aspcts_backend.Models.DTOs.Communication;
using aspcts_backend.Models.Entities;

namespace aspcts_backend.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly ICommunicationRepository _communicationRepository;
        private readonly IChildRepository _childRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CommunicationService(
            ICommunicationRepository communicationRepository,
            IChildRepository childRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _communicationRepository = communicationRepository;
            _childRepository = childRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<MessageResponse> SendMessageAsync(MessageRequest request, Guid senderId)
        {
            // Validate that both sender and receiver have access to the child
            var canSenderAccess = await CanAccessChild(request.ChildId, senderId);
            var canReceiverAccess = await CanAccessChild(request.ChildId, request.ReceiverId);

            if (!canSenderAccess || !canReceiverAccess)
                throw new ArgumentException("Acesso negado para enviar mensagem sobre esta criança");

            var message = new CommunicationMessage
            {
                SenderId = senderId,
                ReceiverId = request.ReceiverId,
                ChildId = request.ChildId,
                Content = request.Content,
                Subject = request.Subject,
                MessageType = request.MessageType,
                Timestamp = DateTime.UtcNow,
                IsRead = false
            };

            await _communicationRepository.AddAsync(message);
            await _communicationRepository.SaveChangesAsync();

            var messages = await _communicationRepository.GetMessagesByChildIdAsync(request.ChildId);
            var sentMessage = messages.FirstOrDefault(m => m.MessageId == message.MessageId);

            return _mapper.Map<MessageResponse>(sentMessage);
        }

        public async Task<IEnumerable<MessageResponse>> GetMessagesByChildIdAsync(Guid childId, Guid userId, string userRole)
        {
            if (!await CanAccessChild(childId, userId))
                return new List<MessageResponse>();

            var messages = await _communicationRepository.GetMessagesByChildIdAsync(childId);
            return messages.Select(m => _mapper.Map<MessageResponse>(m)).ToList();
        }

        public async Task<IEnumerable<MessageResponse>> GetConversationAsync(Guid otherUserId, Guid childId, Guid userId)
        {
            if (!await CanAccessChild(childId, userId))
                return new List<MessageResponse>();

            var messages = await _communicationRepository.GetConversationAsync(userId, otherUserId, childId);
            return messages.Select(m => _mapper.Map<MessageResponse>(m)).ToList();
        }

        public async Task<IEnumerable<MessageResponse>> GetUnreadMessagesAsync(Guid userId)
        {
            var messages = await _communicationRepository.GetUnreadMessagesAsync(userId);
            return messages.Select(m => _mapper.Map<MessageResponse>(m)).ToList();
        }

        public async Task<bool> MarkAsReadAsync(Guid messageId, Guid userId)
        {
            var messages = await _communicationRepository.GetAllAsync();
            var message = messages.FirstOrDefault(m => m.MessageId == messageId);

            if (message == null || message.ReceiverId != userId)
                return false;

            await _communicationRepository.MarkAsReadAsync(messageId);
            return true;
        }

        public async Task<int> GetUnreadCountAsync(Guid userId)
        {
            return await _communicationRepository.GetUnreadCountAsync(userId);
        }

        public async Task<Dictionary<Guid, List<MessageResponse>>> GetConversationSummariesAsync(Guid userId, string userRole)
        {
            var conversations = new Dictionary<Guid, List<MessageResponse>>();

            // Get all children the user has access to
            var children = userRole == "Psychologist"
                ? await GetChildrenForPsychologist(userId)
                : await GetChildrenForParent(userId);

            // Get messages for each child and group by conversation
            foreach (var childId in children)
            {
                var messages = await _communicationRepository.GetMessagesByChildIdAsync(childId);
                var childMessages = messages.Select(m => _mapper.Map<MessageResponse>(m)).ToList();

                if (childMessages.Any())
                {
                    conversations[childId] = childMessages;
                }
            }

            return conversations;
        }

        private async Task<bool> CanAccessChild(Guid childId, Guid userId)
        {
            var child = await _childRepository.GetWithDetailsAsync(childId);
            if (child == null || !child.IsActive)
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || !user.IsActive)
                return false;

            if (user.Role == "Psychologist")
            {
                var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
                return psychologist != null && child.AssignedPsychologistId == psychologist.PsychologistId;
            }
            else if (user.Role == "Parent")
            {
                var parent = await _userRepository.GetParentByUserIdAsync(userId);
                return parent != null &&
                       (child.PrimaryParentId == parent.ParentId || child.SecondaryParentId == parent.ParentId);
            }

            return false;
        }

        private async Task<IEnumerable<Guid>> GetChildrenForPsychologist(Guid userId)
        {
            var psychologist = await _userRepository.GetPsychologistByUserIdAsync(userId);
            if (psychologist == null) return new List<Guid>();

            var children = await _childRepository.GetByPsychologistIdAsync(psychologist.PsychologistId);
            return children.Select(c => c.ChildId);
        }

        private async Task<IEnumerable<Guid>> GetChildrenForParent(Guid userId)
        {
            var parent = await _userRepository.GetParentByUserIdAsync(userId);
            if (parent == null) return new List<Guid>();

            var children = await _childRepository.GetByParentIdAsync(parent.ParentId);
            return children.Select(c => c.ChildId);
        }
    }
}