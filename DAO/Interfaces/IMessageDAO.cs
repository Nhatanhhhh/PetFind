using PetStore.Models.DTOs;
using PetStore.Models.Entities;

namespace PetStore.DAO.Interfaces
{
    public interface IMessageDAO
    {
        Task<MessageResponseDTO?> GetMessageByIdAsync(int id);
        Task<IEnumerable<MessageResponseDTO>> GetMessagesBetweenUsersAsync(
            int senderId,
            int receiverId
        );
        Task<IEnumerable<ConversationDTO>> GetUserConversationsAsync(int userId);
        Task<MessageResponseDTO> CreateMessageAsync(MessageCreateDTO messageDto);
        Task<bool> DeleteMessageAsync(int id);
        Task<bool> MarkAsReadAsync(int id);
        Task<Message?> GetByIdAsync(int id);
        Task AddAsync(Message entity);
        void Update(Message entity);
        void Remove(Message entity);
        Task<int> SaveChangesAsync();
    }
}
