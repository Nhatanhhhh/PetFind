using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PetStore.Data.Context;
using PetStore.Models.Entities;
using PetStore.Models.Enums;

namespace PetStore.Data.SeedData
{
    public static class SeedData
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<User> userManager,
            RoleManager<Role> roleManager
        )
        {
            // 1. Seed các Role mặc định
            var roles = new List<Role>
            {
                new Role { Name = "Admin", Description = "Quản trị viên hệ thống" },
                new Role { Name = "User", Description = "Người dùng thông thường" },
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(role);
                }
            }

            // 2. Seed tài khoản Admin
            var adminUser = await SeedUser(
                userManager,
                "admin@petstore.com",
                "Admin@123",
                "Admin",
                "System",
                "/images/avatars/admin.jpg",
                "0901234567",
                "123 Lê Lợi, Q1, TP.HCM",
                new[] { "Admin" }
            );

            // 3. Seed tài khoản User
            var user = await SeedUser(
                userManager,
                "user@petstore.com",
                "User@123",
                "Nguyễn",
                "Văn A",
                "/images/avatars/user.jpg",
                "0901111222",
                "789 Lý Tự Trọng, Q1, TP.HCM",
                new[] { "User" }
            );

            // 4. Seed các báo cáo thú cưng mẫu
            if (!context.PetReports.Any())
            {
                var petReports = new List<PetReport>
                {
                    new PetReport
                    {
                        UserId = user.Id,
                        Type = "Lost",
                        Species = "Dog",
                        Breed = "Golden Retriever",
                        PetName = "Max",
                        Color = "Vàng",
                        Title = "Mất chó Golden màu vàng",
                        Description =
                            "Chó nhà tôi bị lạc ở công viên 23/9. Có đeo vòng cổ màu đỏ, thân thiện với người.",
                        District = "Quận 1",
                        City = "TP.HCM",
                        Latitude = 10.7769,
                        Longitude = 106.7009,
                        LostOrFoundDate = DateTime.UtcNow.AddDays(-2),
                        Status = Models.Enums.PetReportStatus.Searching,
                        ImageUrl = "/images/pets/dog1.jpg",
                        ContactName = "Nguyễn Văn A",
                        ContactPhone = "0901111222",
                        ContactEmail = "user@petstore.com",
                        IsAISearchEnabled = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                    },
                    new PetReport
                    {
                        UserId = user.Id,
                        Type = "Found",
                        Species = "Cat",
                        Breed = "Mèo ta",
                        Color = "Đen trắng",
                        Title = "Nhặt được mèo đen trắng",
                        Description =
                            "Nhặt được mèo khoảng 3 tháng tuổi ở khu vực chợ Bến Thành. Rất ngoan và sạch sẽ.",
                        District = "Quận 1",
                        City = "TP.HCM",
                        Latitude = 10.7723,
                        Longitude = 106.6980,
                        LostOrFoundDate = DateTime.UtcNow.AddDays(-1),
                        Status = Models.Enums.PetReportStatus.Pending,
                        ImageUrl = "/images/pets/cat1.jpg",
                        ContactName = "Nguyễn Văn A",
                        ContactPhone = "0901111222",
                        ContactEmail = "user@petstore.com",
                        IsAISearchEnabled = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                    },
                };

                await context.PetReports.AddRangeAsync(petReports);
                await context.SaveChangesAsync();
            }

            // 5. Seed các bản kiểm duyệt nội dung (giản lược)
            if (!context.ContentModerations.Any() && context.PetReports.Any())
            {
                var reports = context.PetReports.ToList();
                var moderations = new List<ContentModeration>
                {
                    new ContentModeration
                    {
                        PetReportId = reports[0].Id,
                        ModeratorId = adminUser.Id,
                        Status = Models.Enums.ContentModerationStatus.Approved,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        ReviewedAt = DateTime.UtcNow.AddHours(-12),
                    },
                    new ContentModeration
                    {
                        PetReportId = reports[1].Id,
                        Status = Models.Enums.ContentModerationStatus.Pending,
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                    },
                };

                await context.ContentModerations.AddRangeAsync(moderations);
                await context.SaveChangesAsync();
            }

            // 6. Seed tin nhắn mẫu (giản lược)
            if (!context.Messages.Any() && context.PetReports.Any() && context.Users.Any())
            {
                var reports = context.PetReports.ToList();
                var messages = new List<Message>
                {
                    new Message
                    {
                        SenderId = adminUser.Id,
                        ReceiverId = user.Id,
                        PetReportId = reports[0].Id,
                        Content = "Xin chào, báo cáo của bạn về chú chó Max đã được duyệt.",
                        IsRead = true,
                        CreatedAt = DateTime.UtcNow.AddHours(-5),
                    },
                };

                await context.Messages.AddRangeAsync(messages);
                await context.SaveChangesAsync();
            }
        }

        private static async Task<User> SeedUser(
            UserManager<User> userManager,
            string email,
            string password,
            string firstName,
            string lastName,
            string avatarUrl,
            string phone,
            string address,
            string[] roles
        )
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phone,
                    AvatarUrl = avatarUrl,
                    Address = address,
                    IsActive = true,
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded && roles != null)
                {
                    await userManager.AddToRolesAsync(user, roles);
                }
            }
            return user;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
