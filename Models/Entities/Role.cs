using System.ComponentModel.DataAnnotations;

namespace PetStore.Models.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public ICollection<UserRole> UserRoles { get; set; } = null!;
    }
}
