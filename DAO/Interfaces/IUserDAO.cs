using PetStore.Models.DTOs;
using PetStore.Models.Entities;

namespace PetStore.DAO.Interfaces
{
    public interface IUserDAO
    {
        Task<UserResponseDTO> GetUserByIdAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO> CreateUserAsync(RegisterDTO userDto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO userDto);
        Task<bool> DeleteUserAsync(int id);
        Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(string role);
        Task<Role?> GetRoleByIdAsync(int id);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<Role> CreateRoleAsync(RoleDTO roleDto);
        Task UpdateRoleAsync(RoleDTO roleDto);
        Task DeleteRoleAsync(int id);
        Task<bool> RoleExistsByIdAsync(int id);
        Task<bool> RoleExistsByNameAsync(string name);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByEmailWithRolesAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<bool> UpdateLastLoginAsync(int userId);
    }
}
