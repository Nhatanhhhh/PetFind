using PetStore.Models.DTOs;

namespace PetStore.DAO.Interfaces
{
    public interface IContentModerationDAO
    {
        Task<ContentModerationResponseDTO> GetModerationByIdAsync(int id);
        Task<IEnumerable<ContentModerationResponseDTO>> GetAllModerationsAsync();
        Task<IEnumerable<ContentModerationResponseDTO>> GetModerationsByReportIdAsync(int reportId);
        Task<IEnumerable<ContentModerationResponseDTO>> GetModerationsByModeratorIdAsync(
            int moderatorId
        );
        Task<ContentModerationResponseDTO> CreateModerationAsync(
            ContentModerationCreateDTO moderationDto
        );
        Task<ContentModerationResponseDTO> UpdateModerationAsync(
            int id,
            ContentModerationUpdateDTO moderationDto
        );
        Task<bool> DeleteModerationAsync(int id);
        Task<bool> ModerationExistsAsync(int id);
        Task<bool> ApproveModerationAsync(int moderationId, int moderatorId);
        Task<bool> RejectModerationAsync(int moderationId, int moderatorId, string reason);
    }
}
