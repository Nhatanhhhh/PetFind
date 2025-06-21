using System;
using System.ComponentModel.DataAnnotations;
using PetStore.Models.Enums;

namespace PetStore.Models.Entities
{
    public class ContentModeration
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PetReportId { get; set; }

        public int? ModeratorId { get; set; }

        [Required]
        public ContentModerationStatus Status { get; set; } // Changed from string to enum

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = null;

        public DateTime? ReviewedAt { get; set; }

        // Navigation properties
        public PetReport PetReport { get; set; } = null!;
        public User? Moderator { get; set; }
    }
}
