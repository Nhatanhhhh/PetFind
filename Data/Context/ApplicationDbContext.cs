using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PetStore.Models.Entities;
using PetStore.Models.Enums;

namespace PetStore.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<PetReport> PetReports { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ContentModeration> ContentModerations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User
            builder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(50);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Address).HasMaxLength(200);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
            });

            // Configure Role
            builder.Entity<Role>(entity =>
            {
                entity.Property(r => r.Name).IsRequired().HasMaxLength(50);
                entity.Property(r => r.Description).HasMaxLength(200);
                entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(r => r.Name).IsUnique();
            });

            // Configure UserRole
            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });

                entity
                    .HasOne(ur => ur.User)
                    .WithMany(u => u.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // PetReport configurations
            builder.Entity<PetReport>(entity =>
            {
                entity.Property(p => p.Type).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Species).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Title).IsRequired().HasMaxLength(100);
                entity.Property(p => p.PetName).HasMaxLength(100);
                entity.Property(p => p.Breed).HasMaxLength(50);
                entity.Property(p => p.Color).HasMaxLength(50);
                entity.Property(p => p.Description).IsRequired();
                entity.Property(p => p.District).IsRequired().HasMaxLength(50);
                entity.Property(p => p.City).IsRequired().HasMaxLength(50);
                entity.Property(p => p.Status).HasConversion<string>(); // Convert enum to string
                entity.Property(p => p.ContactName).IsRequired().HasMaxLength(50);
                entity.Property(p => p.ContactPhone).IsRequired();
                entity.Property(p => p.ContactEmail).IsRequired();
                entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity
                    .Property(p => p.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (PetReportStatus)Enum.Parse(typeof(PetReportStatus), v)
                    );
                entity
                    .HasOne(p => p.User)
                    .WithMany(u => u.PetReports)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Message configurations
            builder.Entity<Message>(entity =>
            {
                entity.Property(m => m.Content).IsRequired();
                entity.Property(m => m.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity
                    .HasOne(m => m.Sender)
                    .WithMany(u => u.SentMessages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(m => m.Receiver)
                    .WithMany(u => u.ReceivedMessages)
                    .HasForeignKey(m => m.ReceiverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(m => m.PetReport)
                    .WithMany(p => p.Messages)
                    .HasForeignKey(m => m.PetReportId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ContentModeration configurations
            builder.Entity<ContentModeration>(entity =>
            {
                entity.Property(c => c.Status).HasConversion<string>(); // Convert enum to string
                entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity
                    .Property(c => c.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (ContentModerationStatus)Enum.Parse(typeof(ContentModerationStatus), v)
                    );

                entity
                    .HasOne(c => c.PetReport)
                    .WithMany(p => p.ContentModerations)
                    .HasForeignKey(c => c.PetReportId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasOne(c => c.Moderator)
                    .WithMany()
                    .HasForeignKey(c => c.ModeratorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed Data
            builder
                .Entity<Role>()
                .HasData(
                    new Role
                    {
                        Id = 1,
                        Name = "Admin",
                        Description = "Quản trị viên hệ thống",
                        CreatedAt = DateTime.UtcNow,
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "User",
                        Description = "Người dùng thông thường",
                        CreatedAt = DateTime.UtcNow,
                    }
                );

            string adminPassword = HashPassword("Admin@123");
            string userPassword = HashPassword("User@123");

            builder
                .Entity<User>()
                .HasData(
                    new User
                    {
                        Id = 1,
                        UserName = "admin@petstore.com",
                        Email = "admin@petstore.com",
                        FirstName = "Admin",
                        LastName = "System",
                        PasswordHash = adminPassword,
                        PhoneNumber = "0901234567",
                        AvatarUrl = "/images/avatars/admin.jpg",
                        Address = "123 Lê Lợi, Q1, TP.HCM",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                    },
                    new User
                    {
                        Id = 2,
                        UserName = "user@petstore.com",
                        Email = "user@petstore.com",
                        FirstName = "Nguyễn",
                        LastName = "Văn A",
                        PasswordHash = userPassword,
                        PhoneNumber = "0901111222",
                        AvatarUrl = "/images/avatars/user.jpg",
                        Address = "789 Lý Tự Trọng, Q1, TP.HCM",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                    }
                );

            builder
                .Entity<UserRole>()
                .HasData(
                    new UserRole { UserId = 1, RoleId = 1 },
                    new UserRole { UserId = 2, RoleId = 2 }
                );

            builder
                .Entity<PetReport>()
                .HasData(
                    new PetReport
                    {
                        Id = 1,
                        UserId = 2,
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
                        Status = PetReportStatus.Searching, // Convert to string
                        ImageUrl = "/images/pets/dog1.jpg",
                        ContactName = "Nguyễn Văn A",
                        ContactPhone = "0901111222",
                        ContactEmail = "user@petstore.com",
                        IsAISearchEnabled = true,
                        CreatedAt = DateTime.UtcNow.AddDays(-2),
                    },
                    new PetReport
                    {
                        Id = 2,
                        UserId = 2,
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
                        Status = PetReportStatus.Pending, // Assign enum directly
                        ImageUrl = "/images/pets/cat1.jpg",
                        ContactName = "Nguyễn Văn A",
                        ContactPhone = "0901111222",
                        ContactEmail = "user@petstore.com",
                        IsAISearchEnabled = false,
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                    }
                );

            builder
                .Entity<ContentModeration>()
                .HasData(
                    new ContentModeration
                    {
                        Id = 1,
                        PetReportId = 1,
                        ModeratorId = 1,
                        Status = ContentModerationStatus.Approved, // Assign enum directly
                        CreatedAt = DateTime.UtcNow.AddDays(-1),
                        ReviewedAt = DateTime.UtcNow.AddHours(-12),
                    },
                    new ContentModeration
                    {
                        Id = 2,
                        PetReportId = 2,
                        ModeratorId = 1,
                        Status = ContentModerationStatus.Pending, // Assign enum directly
                        CreatedAt = DateTime.UtcNow.AddHours(-6),
                    }
                );

            builder
                .Entity<Message>()
                .HasData(
                    new Message
                    {
                        Id = 1,
                        SenderId = 1,
                        ReceiverId = 2,
                        PetReportId = 1,
                        Content = "Xin chào, báo cáo của bạn về chú chó Max đã được duyệt.",
                        IsRead = true,
                        CreatedAt = DateTime.UtcNow.AddHours(-5),
                    }
                );
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
