using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PetStore.DAO.Interfaces;
using PetStore.Data.Context;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;

namespace PetStore.DAO
{
    public class UserDAO : IUserDAO
    {
        private readonly ApplicationDbContext _context;

        public UserDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByEmailWithRolesAsync(string email)
        {
            return await _context
                .Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<UserResponseDTO> GetUserByIdAsync(int id)
        {
            var user = await _context
                .Users.Include(u => u.PetReports)
                .Include(u => u.SentMessages)
                .Include(u => u.ReceivedMessages)
                .Include(u => u.Moderations)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            return MapToDTO(user);
        }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _context
                .Users.Include(u => u.PetReports)
                .Include(u => u.SentMessages)
                .Include(u => u.ReceivedMessages)
                .Include(u => u.Moderations)
                .ToListAsync();

            return users.Select(MapToDTO);
        }

        public async Task<UserResponseDTO> CreateUserAsync(RegisterDTO userDto)
        {
            var user = new User
            {
                UserName = userDto.Email, // Sử dụng Email làm UserName
                Email = userDto.Email,
                FirstName = userDto.FirstName ?? string.Empty,
                LastName = userDto.LastName ?? string.Empty,
                PhoneNumber = userDto.PhoneNumber,
                Address = userDto.Address ?? string.Empty,
                PasswordHash = HashPassword(userDto.Password),
                IsActive = true, // Mặc định kích hoạt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Gán vai trò mặc định là "User"
            var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (defaultRole != null)
            {
                _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = defaultRole.Id });
                await _context.SaveChangesAsync();
            }

            return MapToDTO(user);
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {id} not found.");

            // Update only the properties that are provided in the DTO
            user.FirstName = userDto.FirstName ?? user.FirstName;
            user.LastName = userDto.LastName ?? user.LastName;
            user.PhoneNumber = userDto.PhoneNumber ?? user.PhoneNumber;
            user.Address = userDto.Address ?? user.Address;
            user.AvatarUrl = userDto.AvatarUrl ?? user.AvatarUrl;

            // Update other properties
            user.IsActive = userDto.IsActive;
            if (userDto.LastLoginAt != default)
                user.LastLoginAt = userDto.LastLoginAt;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<UserResponseDTO>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
                throw new ArgumentException("Role name cannot be null or empty", nameof(role));

            var users = await _context
                .Users.Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Include(u => u.PetReports)
                .Include(u => u.SentMessages)
                .Include(u => u.ReceivedMessages)
                .Include(u => u.Moderations)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == role))
                .ToListAsync();

            return users.Select(MapToDTO);
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _context.Roles.FindAsync(id);
        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> CreateRoleAsync(RoleDTO roleDto)
        {
            if (roleDto == null)
                throw new ArgumentNullException(nameof(roleDto));

            if (await RoleExistsByNameAsync(roleDto.Name))
                throw new InvalidOperationException(
                    $"Role with name '{roleDto.Name}' already exists."
                );

            var role = new Role { Name = roleDto.Name, Description = roleDto.Description };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateRoleAsync(RoleDTO roleDto)
        {
            if (roleDto == null)
                throw new ArgumentNullException(nameof(roleDto));

            var role = await _context.Roles.FindAsync(roleDto.Id);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {roleDto.Id} not found.");

            role.Name = roleDto.Name;
            role.Description = roleDto.Description;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                throw new KeyNotFoundException($"Role with ID {id} not found.");

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RoleExistsByIdAsync(int id)
        {
            return await _context.Roles.AnyAsync(r => r.Id == id);
        }

        public async Task<bool> RoleExistsByNameAsync(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Role name cannot be null or empty", nameof(name));

            return await _context.Roles.AnyAsync(r => r.Name == name);
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastLoginAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private static UserResponseDTO MapToDTO(User user)
        {
            return new UserResponseDTO
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Address = user.Address ?? string.Empty,
                AvatarUrl = user.AvatarUrl ?? string.Empty,
                IsActive = user.IsActive,
                PetReports =
                    user.PetReports?.Select(pr => new PetReportResponseDTO
                        {
                            Id = pr.Id,
                            UserId = pr.UserId,
                            Type = pr.Type ?? string.Empty,
                            Species = pr.Species ?? string.Empty,
                            Title = pr.Title ?? string.Empty,
                            Description = pr.Description ?? string.Empty,
                            District = pr.District ?? string.Empty,
                            City = pr.City ?? string.Empty,
                            Status = pr.Status,
                            CreatedAt = pr.CreatedAt,
                        })
                        .ToList() ?? new List<PetReportResponseDTO>(),
                SentMessages =
                    user.SentMessages?.Select(m => new MessageResponseDTO
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            ReceiverId = m.ReceiverId,
                            Content = m.Content ?? string.Empty,
                            IsRead = m.IsRead,
                            CreatedAt = m.CreatedAt,
                        })
                        .ToList() ?? new List<MessageResponseDTO>(),
                ReceivedMessages =
                    user.ReceivedMessages?.Select(m => new MessageResponseDTO
                        {
                            Id = m.Id,
                            SenderId = m.SenderId,
                            ReceiverId = m.ReceiverId,
                            Content = m.Content ?? string.Empty,
                            IsRead = m.IsRead,
                            CreatedAt = m.CreatedAt,
                        })
                        .ToList() ?? new List<MessageResponseDTO>(),
                Moderations =
                    user.Moderations?.Select(cm => new ContentModerationResponseDTO
                        {
                            Id = cm.Id,
                            PetReportId = cm.PetReportId,
                            ModeratorId = cm.ModeratorId,
                            Status = cm.Status,
                            RejectionReason = cm.RejectionReason,
                            CreatedAt = cm.CreatedAt,
                            ReviewedAt = cm.ReviewedAt,
                        })
                        .ToList() ?? new List<ContentModerationResponseDTO>(),
            };
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
