using PetStore.Models.DTOs;
using PetStore.Models.Enums;

namespace PetStore.DAO.Interfaces
{
    public interface IPetReportDAO
    {
        Task<PetReportResponseDTO> GetReportByIdAsync(int id);
        Task<IEnumerable<PetReportResponseDTO>> GetAllReportsAsync();
        Task<IEnumerable<PetReportResponseDTO>> GetReportsByTypeAsync(string type);
        Task<IEnumerable<PetReportResponseDTO>> GetReportsByUserAsync(int userId);
        Task<IEnumerable<PetReportResponseDTO>> SearchReportsAsync(PetReportSearchDTO searchDto);
        Task<PetReportResponseDTO> CreateReportAsync(PetReportCreateDTO reportDto);
        Task<PetReportResponseDTO> UpdateReportAsync(int id, PetReportUpdateDTO reportDto);
        Task<bool> DeleteReportAsync(int id);
        Task<bool> UpdateStatusAsync(int id, PetReportStatus status);
        Task<IEnumerable<PetReportResponseDTO>> GetPendingReportsAsync();
    }
}
