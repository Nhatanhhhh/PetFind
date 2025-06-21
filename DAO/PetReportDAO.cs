using Microsoft.EntityFrameworkCore;
using PetStore.DAO.Interfaces;
using PetStore.Data.Context;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;
using PetStore.Models.Enums;

namespace PetStore.DAO
{
    public class PetReportDAO : IPetReportDAO
    {
        private readonly ApplicationDbContext _context;

        public PetReportDAO(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PetReportResponseDTO> GetReportByIdAsync(int id)
        {
            var report = await _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
                throw new KeyNotFoundException($"Pet report with ID {id} not found");

            return new PetReportResponseDTO
            {
                Id = report.Id,
                UserId = report.UserId,
                Type = report.Type,
                Species = report.Species,
                Breed = report.Breed,
                Color = report.Color,
                PetName = report.PetName,
                Title = report.Title,
                Description = report.Description,
                District = report.District,
                City = report.City,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                LostOrFoundDate = report.LostOrFoundDate,
                CreatedAt = report.CreatedAt,
                Status = report.Status,
                ImageUrl = report.ImageUrl,
                ContactName = report.ContactName,
                ContactPhone = report.ContactPhone,
                ContactEmail = report.ContactEmail,
                ContactNote = report.ContactNote,
                IsAISearchEnabled = report.IsAISearchEnabled,
                User =
                    report.User != null
                        ? new UserResponseDTO
                        {
                            Id = report.User.Id,
                            UserName = report.User.UserName,
                            Email = report.User.Email,
                            FirstName = report.User.FirstName,
                            LastName = report.User.LastName,
                        }
                        : new UserResponseDTO(),
                Moderations = report
                    .ContentModerations.Select(m => new ContentModerationResponseDTO
                    {
                        Id = m.Id,
                        PetReportId = m.PetReportId,
                        ModeratorId = m.ModeratorId,
                        Status = m.Status,
                        RejectionReason = m.RejectionReason,
                        CreatedAt = m.CreatedAt,
                        ReviewedAt = m.ReviewedAt,
                    })
                    .ToList(),
            };
        }

        public async Task<IEnumerable<PetReportResponseDTO>> GetAllReportsAsync()
        {
            var reports = await _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PetReportResponseDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Type = r.Type,
                    Species = r.Species,
                    Breed = r.Breed,
                    Color = r.Color,
                    PetName = r.PetName,
                    Title = r.Title,
                    Description = r.Description,
                    District = r.District,
                    City = r.City,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    LostOrFoundDate = r.LostOrFoundDate,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status,
                    ImageUrl = r.ImageUrl,
                    ContactName = r.ContactName,
                    ContactPhone = r.ContactPhone,
                    ContactEmail = r.ContactEmail,
                    ContactNote = r.ContactNote,
                    IsAISearchEnabled = r.IsAISearchEnabled,
                    User = new UserResponseDTO
                    {
                        Id = r.User.Id,
                        UserName = r.User.UserName,
                        Email = r.User.Email,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName,
                    },
                    Moderations = r
                        .ContentModerations.Select(m => new ContentModerationResponseDTO
                        {
                            Id = m.Id,
                            PetReportId = m.PetReportId,
                            ModeratorId = m.ModeratorId,
                            Status = m.Status,
                            RejectionReason = m.RejectionReason,
                            CreatedAt = m.CreatedAt,
                            ReviewedAt = m.ReviewedAt,
                        })
                        .ToList(),
                })
                .ToListAsync();
            return reports;
        }

        public async Task<IEnumerable<PetReportResponseDTO>> GetReportsByTypeAsync(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentException("Type cannot be null or empty", nameof(type));

            var reports = await _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .Where(r => r.Type == type)
                .ToListAsync();

            return reports.Select(MapToDTO);
        }

        public async Task<IEnumerable<PetReportResponseDTO>> GetReportsByUserAsync(int userId)
        {
            var reports = await _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .Where(r => r.UserId == userId)
                .ToListAsync();

            return reports.Select(MapToDTO);
        }

        public async Task<IEnumerable<PetReportResponseDTO>> SearchReportsAsync(
            PetReportSearchDTO searchDto
        )
        {
            var query = _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.Type))
                query = query.Where(r => r.Type == searchDto.Type);

            if (!string.IsNullOrEmpty(searchDto.Species))
                query = query.Where(r => r.Species == searchDto.Species);

            if (!string.IsNullOrEmpty(searchDto.City))
                query = query.Where(r => r.City == searchDto.City);

            if (!string.IsNullOrEmpty(searchDto.District))
                query = query.Where(r => r.District == searchDto.District);

            if (searchDto.LostOrFoundDate.HasValue)
            {
                var date = searchDto.LostOrFoundDate.Value.Date;
                query = query.Where(r => r.LostOrFoundDate.Date == date);
            }

            var reports = await query.ToListAsync();
            return reports.Select(MapToDTO);
        }

        public async Task<PetReportResponseDTO> CreateReportAsync(PetReportCreateDTO reportDto)
        {
            var report = new PetReport
            {
                UserId = reportDto.UserId,
                Type = reportDto.Type,
                Species = reportDto.Species,
                Title = reportDto.Title,
                PetName = reportDto.PetName,
                Breed = reportDto.Breed,
                Color = reportDto.Color,
                Description = reportDto.Description,
                District = reportDto.District,
                City = reportDto.City,
                LostOrFoundDate = reportDto.LostOrFoundDate,
                Status = PetReportStatus.Pending,
                ContactName = reportDto.ContactName,
                ContactPhone = reportDto.ContactPhone,
                ContactEmail = reportDto.ContactEmail,
                ContactNote = reportDto.ContactNote,
                ImageUrl = reportDto.ImageUrl,
                IsAISearchEnabled = reportDto.IsAISearchEnabled,
                Latitude = reportDto.Latitude,
                Longitude = reportDto.Longitude,
                CreatedAt = DateTime.UtcNow,
            };

            _context.PetReports.Add(report);
            await _context.SaveChangesAsync();

            var moderation = new ContentModeration
            {
                PetReportId = report.Id,
                ModeratorId = null,
                Status = ContentModerationStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            _context.ContentModerations.Add(moderation);
            await _context.SaveChangesAsync();

            return await GetReportByIdAsync(report.Id);
        }

        public async Task<PetReportResponseDTO> UpdateReportAsync(
            int id,
            PetReportUpdateDTO reportDto
        )
        {
            var report = await _context.PetReports.FindAsync(id);
            if (report == null)
                throw new KeyNotFoundException($"Pet report with ID {id} not found");

            report.Type = reportDto.Type ?? report.Type;
            report.Species = reportDto.Species ?? report.Species;
            report.Title = reportDto.Title ?? report.Title;
            report.PetName = reportDto.PetName ?? report.PetName;
            report.Breed = reportDto.Breed ?? report.Breed;
            report.Color = reportDto.Color ?? report.Color;
            report.Description = reportDto.Description ?? report.Description;
            report.District = reportDto.District ?? report.District;
            report.City = reportDto.City ?? report.City;
            report.LostOrFoundDate = reportDto.LostOrFoundDate;
            report.Status = reportDto.Status;
            report.ContactName = reportDto.ContactName ?? report.ContactName;
            report.ContactPhone = reportDto.ContactPhone ?? report.ContactPhone;
            report.Latitude = reportDto.Latitude ?? report.Latitude;
            report.Longitude = reportDto.Longitude ?? report.Longitude;
            report.ImageUrl = reportDto.ImageUrl ?? report.ImageUrl;
            report.ContactEmail = reportDto.ContactEmail ?? report.ContactEmail;
            report.ContactNote = reportDto.ContactNote ?? report.ContactNote;
            report.IsAISearchEnabled = reportDto.IsAISearchEnabled;

            await _context.SaveChangesAsync();

            return await GetReportByIdAsync(id);
        }

        public async Task<bool> DeleteReportAsync(int id)
        {
            var report = await _context.PetReports.FindAsync(id);
            if (report == null)
                return false;

            _context.PetReports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, PetReportStatus status)
        {
            var report = await _context.PetReports.FindAsync(id);
            if (report == null)
                return false;

            report.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<PetReportResponseDTO>> GetPendingReportsAsync()
        {
            var reports = await _context
                .PetReports.Include(r => r.User)
                .Include(r => r.ContentModerations)
                .Where(r => r.Status == PetReportStatus.Pending)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new PetReportResponseDTO
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    Type = r.Type,
                    Species = r.Species,
                    Breed = r.Breed,
                    Color = r.Color,
                    PetName = r.PetName,
                    Title = r.Title,
                    Description = r.Description,
                    District = r.District,
                    City = r.City,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    LostOrFoundDate = r.LostOrFoundDate,
                    CreatedAt = r.CreatedAt,
                    Status = r.Status,
                    ImageUrl = r.ImageUrl,
                    ContactName = r.ContactName,
                    ContactPhone = r.ContactPhone,
                    ContactEmail = r.ContactEmail,
                    ContactNote = r.ContactNote,
                    IsAISearchEnabled = r.IsAISearchEnabled,
                    User = new UserResponseDTO
                    {
                        Id = r.User.Id,
                        UserName = r.User.UserName,
                        Email = r.User.Email,
                        FirstName = r.User.FirstName,
                        LastName = r.User.LastName,
                    },
                    Moderations = r
                        .ContentModerations.Select(m => new ContentModerationResponseDTO
                        {
                            Id = m.Id,
                            PetReportId = m.PetReportId,
                            ModeratorId = m.ModeratorId,
                            Status = m.Status,
                            CreatedAt = m.CreatedAt,
                        })
                        .ToList(),
                })
                .ToListAsync();
            return reports;
        }

        private static PetReportResponseDTO MapToDTO(PetReport report)
        {
            return new PetReportResponseDTO
            {
                Id = report.Id,
                UserId = report.UserId,
                Type = report.Type,
                Species = report.Species,
                Breed = report.Breed,
                Color = report.Color,
                PetName = report.PetName,
                Title = report.Title,
                Description = report.Description,
                District = report.District,
                City = report.City,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
                LostOrFoundDate = report.LostOrFoundDate,
                CreatedAt = report.CreatedAt,
                Status = report.Status,
                ImageUrl = report.ImageUrl,
                ContactName = report.ContactName,
                ContactPhone = report.ContactPhone,
                ContactEmail = report.ContactEmail,
                ContactNote = report.ContactNote,
                IsAISearchEnabled = report.IsAISearchEnabled,
                User =
                    report.User != null
                        ? new UserResponseDTO
                        {
                            Id = report.User.Id,
                            Email = report.User.Email,
                            FirstName = report.User.FirstName,
                            LastName = report.User.LastName,
                            PhoneNumber = report.User.PhoneNumber,
                            Address = report.User.Address,
                            AvatarUrl = report.User.AvatarUrl,
                            IsActive = report.User.IsActive,
                        }
                        : new UserResponseDTO(),
                Moderations =
                    report
                        .ContentModerations?.Select(m => new ContentModerationResponseDTO
                        {
                            Id = m.Id,
                            PetReportId = m.PetReportId,
                            ModeratorId = m.ModeratorId,
                            Status = m.Status,
                            RejectionReason = m.RejectionReason,
                            CreatedAt = m.CreatedAt,
                            ReviewedAt = m.ReviewedAt,
                        })
                        .ToList() ?? new List<ContentModerationResponseDTO>(),
            };
        }
    }
}
