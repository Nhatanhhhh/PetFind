using Microsoft.EntityFrameworkCore;
using PetStore.DAO.Interfaces;
using PetStore.Data.Context;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;
using PetStore.Models.Enums;

namespace PetStore.DAO
{
    public class ContentModerationDAO : IContentModerationDAO
    {
        private readonly ApplicationDbContext _context;

        public ContentModerationDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ContentModerationResponseDTO> GetModerationByIdAsync(int id)
        {
            var moderation = await _context
                .ContentModerations.Include(m => m.PetReport)
                .Include(m => m.Moderator)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (moderation == null)
                throw new KeyNotFoundException($"Content moderation with ID {id} not found");

            return MapToDTO(moderation);
        }

        public async Task<IEnumerable<ContentModerationResponseDTO>> GetAllModerationsAsync()
        {
            var moderations = await _context
                .ContentModerations.Include(m => m.PetReport)
                .Include(m => m.Moderator)
                .ToListAsync();

            return moderations.Select(MapToDTO);
        }

        public async Task<IEnumerable<ContentModerationResponseDTO>> GetModerationsByReportIdAsync(
            int reportId
        )
        {
            var moderations = await _context
                .ContentModerations.Include(m => m.PetReport)
                .Include(m => m.Moderator)
                .Where(m => m.PetReportId == reportId)
                .ToListAsync();

            return moderations.Select(MapToDTO);
        }

        public async Task<
            IEnumerable<ContentModerationResponseDTO>
        > GetModerationsByModeratorIdAsync(int moderatorId)
        {
            var moderations = await _context
                .ContentModerations.Include(m => m.PetReport)
                .Include(m => m.Moderator)
                .Where(m => m.ModeratorId == moderatorId)
                .ToListAsync();

            return moderations.Select(MapToDTO);
        }

        public async Task<ContentModerationResponseDTO> CreateModerationAsync(
            ContentModerationCreateDTO moderationDto
        )
        {
            var moderation = new ContentModeration
            {
                PetReportId = moderationDto.PetReportId,
                ModeratorId = moderationDto.ModeratorId,
                Status = moderationDto.Status,
                RejectionReason = moderationDto.RejectionReason,
                CreatedAt = DateTime.UtcNow,
            };

            _context.ContentModerations.Add(moderation);
            await _context.SaveChangesAsync();

            return await GetModerationByIdAsync(moderation.Id);
        }

        public async Task<ContentModerationResponseDTO> UpdateModerationAsync(
            int id,
            ContentModerationUpdateDTO moderationDto
        )
        {
            var moderation = await _context.ContentModerations.FindAsync(id);
            if (moderation == null)
                throw new KeyNotFoundException($"Content moderation with ID {id} not found");

            moderation.Status = moderationDto.Status;
            moderation.RejectionReason = moderationDto.RejectionReason;
            moderation.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetModerationByIdAsync(id);
        }

        public async Task<bool> DeleteModerationAsync(int id)
        {
            var moderation = await _context.ContentModerations.FindAsync(id);
            if (moderation == null)
                return false;

            _context.ContentModerations.Remove(moderation);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ModerationExistsAsync(int id)
        {
            return await _context.ContentModerations.AnyAsync(m => m.Id == id);
        }

        private static ContentModerationResponseDTO MapToDTO(ContentModeration moderation)
        {
            return new ContentModerationResponseDTO
            {
                Id = moderation.Id,
                PetReportId = moderation.PetReportId,
                ModeratorId = moderation.ModeratorId,
                Status = moderation.Status,
                RejectionReason = moderation.RejectionReason,
                CreatedAt = moderation.CreatedAt,
                ReviewedAt = moderation.ReviewedAt,
                PetReport =
                    moderation.PetReport != null
                        ? new PetReportResponseDTO
                        {
                            Id = moderation.PetReport.Id,
                            UserId = moderation.PetReport.UserId,
                            Type = moderation.PetReport.Type,
                            Species = moderation.PetReport.Species,
                            Title = moderation.PetReport.Title,
                            Description = moderation.PetReport.Description,
                            District = moderation.PetReport.District,
                            City = moderation.PetReport.City,
                            Status = moderation.PetReport.Status,
                            CreatedAt = moderation.PetReport.CreatedAt,
                        }
                        : new PetReportResponseDTO(),
                Moderator =
                    moderation.Moderator != null
                        ? new UserResponseDTO
                        {
                            Id = moderation.Moderator.Id,
                            Email = moderation.Moderator.Email,
                            FirstName = moderation.Moderator.FirstName,
                            LastName = moderation.Moderator.LastName,
                            PhoneNumber = moderation.Moderator.PhoneNumber,
                            Address = moderation.Moderator.Address,
                            AvatarUrl = moderation.Moderator.AvatarUrl,
                            IsActive = moderation.Moderator.IsActive,
                        }
                        : null,
            };
        }

        public async Task<bool> ApproveModerationAsync(int moderationId, int moderatorId)
        {
            try
            {
                var moderation = await _context
                    .ContentModerations.Include(m => m.PetReport)
                    .FirstOrDefaultAsync(m => m.Id == moderationId);

                if (moderation == null)
                    return false;

                // Cập nhật moderation status
                moderation.Status = ContentModerationStatus.Approved;
                moderation.ModeratorId = moderatorId;
                moderation.ReviewedAt = DateTime.UtcNow;

                // Cập nhật PetReport status thành Searching (đã được duyệt)
                if (moderation.PetReport != null)
                {
                    moderation.PetReport.Status = PetReportStatus.Searching;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RejectModerationAsync(
            int moderationId,
            int moderatorId,
            string reason
        )
        {
            try
            {
                var moderation = await _context
                    .ContentModerations.Include(m => m.PetReport)
                    .FirstOrDefaultAsync(m => m.Id == moderationId);

                if (moderation == null)
                    return false;

                // Cập nhật moderation status
                moderation.Status = ContentModerationStatus.Rejected;
                moderation.ModeratorId = moderatorId;
                moderation.RejectionReason = reason;
                moderation.ReviewedAt = DateTime.UtcNow;

                // Cập nhật PetReport status thành Rejected
                if (moderation.PetReport != null)
                {
                    moderation.PetReport.Status = PetReportStatus.Rejected;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
