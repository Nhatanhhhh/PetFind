using Microsoft.EntityFrameworkCore;
using PetStore.DAO.Interfaces;
using PetStore.Data.Context;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;

namespace PetStore.DAO
{
    public class MessageDAO : IMessageDAO
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Message> _dbSet;

        public MessageDAO(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Message>();
        }

        public async Task<MessageResponseDTO?> GetMessageByIdAsync(int id)
        {
            var message = await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.PetReport)
                .FirstOrDefaultAsync(m => m.Id == id);
            return message == null ? null : MapToResponseDTO(message);
        }

        public async Task<IEnumerable<MessageResponseDTO>> GetMessagesBetweenUsersAsync(
            int senderId,
            int receiverId
        )
        {
            var messages = await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.PetReport)
                .Where(m =>
                    (m.SenderId == senderId && m.ReceiverId == receiverId)
                    || (m.SenderId == receiverId && m.ReceiverId == senderId)
                )
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
            return messages.Select(MapToResponseDTO);
        }

        public async Task<IEnumerable<ConversationDTO>> GetUserConversationsAsync(int userId)
        {
            var conversations = await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .GroupBy(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Select(g => new ConversationDTO
                {
                    UserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.CreatedAt).First().Content,
                    Timestamp = g.Max(m => m.CreatedAt),
                    OtherUser =
                        g.First().SenderId == userId
                            ? new UserResponseDTO
                            {
                                Id = g.First().Receiver.Id,
                                Email = g.First().Receiver.Email,
                                FirstName = g.First().Receiver.FirstName,
                                LastName = g.First().Receiver.LastName,
                                PhoneNumber = g.First().Receiver.PhoneNumber,
                                Address = g.First().Receiver.Address,
                                AvatarUrl = g.First().Receiver.AvatarUrl,
                                IsActive = g.First().Receiver.IsActive,
                            }
                            : new UserResponseDTO
                            {
                                Id = g.First().Sender.Id,
                                Email = g.First().Sender.Email,
                                FirstName = g.First().Sender.FirstName,
                                LastName = g.First().Sender.LastName,
                                PhoneNumber = g.First().Sender.PhoneNumber,
                                Address = g.First().Sender.Address,
                                AvatarUrl = g.First().Sender.AvatarUrl,
                                IsActive = g.First().Sender.IsActive,
                            },
                })
                .ToListAsync();
            return conversations;
        }

        public async Task<MessageResponseDTO> CreateMessageAsync(MessageCreateDTO messageDto)
        {
            var message = new Message
            {
                SenderId = messageDto.SenderId,
                ReceiverId = messageDto.ReceiverId,
                Content = messageDto.Content,
                CreatedAt = DateTime.UtcNow,
                IsRead = false,
                PetReportId = messageDto.PetReportId,
            };
            await _dbSet.AddAsync(message);
            await SaveChangesAsync();
            return MapToResponseDTO(message);
        }

        public async Task<bool> DeleteMessageAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message == null)
                return false;
            _dbSet.Remove(message);
            await SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsReadAsync(int id)
        {
            var message = await GetByIdAsync(id);
            if (message == null)
                return false;
            message.IsRead = true;
            _dbSet.Update(message);
            await SaveChangesAsync();
            return true;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.PetReport)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Message entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(Message entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(Message entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private MessageResponseDTO MapToResponseDTO(Message message)
        {
            return new MessageResponseDTO
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                CreatedAt = message.CreatedAt,
                IsRead = message.IsRead,
                PetReportId = message.PetReportId ?? 0,
                Sender =
                    message.Sender != null
                        ? new UserResponseDTO
                        {
                            Id = message.Sender.Id,
                            Email = message.Sender.Email,
                            FirstName = message.Sender.FirstName,
                            LastName = message.Sender.LastName,
                            PhoneNumber = message.Sender.PhoneNumber,
                            Address = message.Sender.Address,
                            AvatarUrl = message.Sender.AvatarUrl,
                            IsActive = message.Sender.IsActive,
                        }
                        : new UserResponseDTO(),
                Receiver =
                    message.Receiver != null
                        ? new UserResponseDTO
                        {
                            Id = message.Receiver.Id,
                            Email = message.Receiver.Email,
                            FirstName = message.Receiver.FirstName,
                            LastName = message.Receiver.LastName,
                            PhoneNumber = message.Receiver.PhoneNumber,
                            Address = message.Receiver.Address,
                            AvatarUrl = message.Receiver.AvatarUrl,
                            IsActive = message.Receiver.IsActive,
                        }
                        : new UserResponseDTO(),
                PetReport =
                    message.PetReport != null
                        ? new PetReportResponseDTO
                        {
                            Id = message.PetReport.Id,
                            UserId = message.PetReport.UserId,
                            Type = message.PetReport.Type,
                            Species = message.PetReport.Species,
                            Title = message.PetReport.Title,
                            Description = message.PetReport.Description,
                            District = message.PetReport.District,
                            City = message.PetReport.City,
                            Status = message.PetReport.Status,
                            CreatedAt = message.PetReport.CreatedAt,
                        }
                        : new PetReportResponseDTO(),
            };
        }
    }
}
