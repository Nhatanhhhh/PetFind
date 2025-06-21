namespace PetStore.Models.DTOs
{
    public class ActivityDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public UserResponseDTO? User { get; set; }
    }
}
