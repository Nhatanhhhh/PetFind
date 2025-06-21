using System.ComponentModel.DataAnnotations;

namespace PetStore.Models.DTOs
{
    public class MessageResponseDTO
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public int? PetReportId { get; set; }
        public UserResponseDTO Sender { get; set; } = new();
        public UserResponseDTO Receiver { get; set; } = new();
        public PetReportResponseDTO PetReport { get; set; } = new();
    }

    public class MessageCreateDTO
    {
        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }
        public int? PetReportId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;
    }

    public class MessageUpdateDTO
    {
        [Required]
        public string Content { get; set; } = string.Empty;
    }

    public class ConversationDTO
    {
        public int UserId { get; set; }
        public string LastMessage { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public UserResponseDTO OtherUser { get; set; } = new();
    }
}
