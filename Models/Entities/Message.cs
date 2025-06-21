using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? PetReportId { get; set; }

        // Navigation properties
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
        public PetReport? PetReport { get; set; }
    }
}
