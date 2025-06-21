using System.ComponentModel.DataAnnotations;
using PetStore.Models.Enums;

namespace PetStore.Models.Entities
{
    public class PetReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // Lost or Found (consider enum if needed)

        [Required]
        [StringLength(50)]
        public string Species { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PetName { get; set; }

        [StringLength(50)]
        public string? Breed { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Address { get; set; }

        [Required]
        [StringLength(100)]
        public string District { get; set; } = "Ba Dinh";

        [Required]
        [StringLength(100)]
        public string City { get; set; } = "Hanoi";

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        [Required]
        public DateTime LostOrFoundDate { get; set; }

        [Required]
        public PetReportStatus Status { get; set; } = PetReportStatus.Searching; // Changed from string to enum

        [StringLength(100)]
        public string? ImageUrl { get; set; }

        [Required]
        [StringLength(100)]
        public string ContactName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string ContactPhone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(50)]
        public string ContactEmail { get; set; } = string.Empty;

        public string? ContactNote { get; set; }

        public bool IsAISearchEnabled { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<ContentModeration> ContentModerations { get; set; } =
            new List<ContentModeration>();
    }
}
