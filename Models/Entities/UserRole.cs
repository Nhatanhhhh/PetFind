using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetStore.Models.Entities
{
    public class UserRole
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int RoleId { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;

        [ForeignKey("RoleId")]
        public Role Role { get; set; } = null!;
    }
}
