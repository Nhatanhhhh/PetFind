using System.ComponentModel.DataAnnotations;
using PetStore.Models.Enums;

namespace PetStore.Models.DTOs
{
    public class PetReportResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty; // Could use PetReportType enum
        public string Species { get; set; } = string.Empty;
        public string? PetName { get; set; }
        public string? Breed { get; set; }
        public string? Color { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? LostOrFoundDate { get; set; } // Nullable to handle potential nulls
        public DateTime CreatedAt { get; set; }
        public PetReportStatus Status { get; set; }
        public string? ImageUrl { get; set; }
        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactNote { get; set; }
        public bool IsAISearchEnabled { get; set; }
        public UserResponseDTO? User { get; set; }
        public List<ContentModerationResponseDTO> Moderations { get; set; } = new();
    }

    public class PetReportCreateDTO
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [Required]
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

        [Required]
        public DateTime LostOrFoundDate { get; set; }
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
        public IFormFile? ImageFile { get; set; }
        public bool IsAISearchEnabled { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class PetReportUpdateDTO
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } = "Lost";

        [Required]
        public string Species { get; set; } = "Dog";

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? PetName { get; set; }

        [StringLength(50)]
        public string? Breed { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
        public string? Address { get; set; }

        [Required]
        public string District { get; set; } = "Ba Dinh";

        [Required]
        public string City { get; set; } = "Hanoi";

        [Required]
        public DateTime LostOrFoundDate { get; set; }

        [Required]
        public PetReportStatus Status { get; set; } = PetReportStatus.Searching;
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
        public IFormFile? ImageFile { get; set; }
        public bool IsAISearchEnabled { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }

    public class PetReportSearchDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string District { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public PetReportStatus? Status { get; set; } // Nullable enum
        public string? Type { get; set; }
        public string? Species { get; set; }
        public DateTime? LostOrFoundDate { get; set; }
    }
}
