using System.ComponentModel.DataAnnotations;
using PetStore.Models.Enums;

namespace PetStore.Models.DTOs
{
    public class ContentModerationResponseDTO
    {
        public int Id { get; set; }
        public int PetReportId { get; set; }
        public int? ModeratorId { get; set; }
        public ContentModerationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? Comment { get; set; }
        public PetReportResponseDTO PetReport { get; set; } = new();
        public UserResponseDTO? Moderator { get; set; }
    }

    public class ContentModerationCreateDTO
    {
        [Required]
        public int PetReportId { get; set; }

        [Required]
        public int ModeratorId { get; set; }

        [Required]
        public ContentModerationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
        public string? Comment { get; set; }
    }

    public class ContentModerationUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public ContentModerationStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}
