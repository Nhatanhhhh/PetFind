using System.Collections.Generic;
using PetStore.Models.Enums;

namespace PetStore.Models.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalReports { get; set; }
        public int PendingModerations { get; set; }
        public int TotalUsers { get; set; }
        public int TotalMessages { get; set; }
        public int LostReportsCount { get; set; }
        public int FoundReportsCount { get; set; }
        public int ActiveUsersCount { get; set; }
        public int InactiveUsersCount { get; set; }
        public List<PetReportResponseDTO> PendingReports { get; set; } = new();
        public List<ContentModerationResponseDTO> RecentActivities { get; set; } = new();
        public List<UserResponseDTO> RecentUsers { get; set; } = new();
        public Dictionary<string, int> ReportsByStatus { get; set; } = new();
        public Dictionary<string, int> ReportsByType { get; set; } = new();
        public Dictionary<string, int> ReportsByCity { get; set; } = new();
    }

    public class AdminUserManagementDTO
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<string> Roles { get; set; } = new();
        public int ReportsCount { get; set; }
        public int MessagesCount { get; set; }
    }

    public class AdminReportManagementDTO
    {
        public int ReportId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Species { get; set; } = string.Empty;
        public PetReportStatus Status { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ContentModerationStatus? ModerationStatus { get; set; }
        public string? RejectionReason { get; set; }
        public int ViewsCount { get; set; }
        public int MessagesCount { get; set; }
    }

    public class AdminModerationDTO
    {
        public int ModerationId { get; set; }
        public int PetReportId { get; set; }
        public string ReportTitle { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ModeratorName { get; set; } = string.Empty;
        public ContentModerationStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectionReason { get; set; }
        public string ReportDescription { get; set; } = string.Empty;
    }

    public class AdminStatisticsDTO
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalReports { get; set; }
        public int PendingReports { get; set; }
        public int ApprovedReports { get; set; }
        public int RejectedReports { get; set; }
        public int TotalMessages { get; set; }
        public Dictionary<string, int> ReportsByMonth { get; set; } = new();
        public Dictionary<string, int> UsersByMonth { get; set; } = new();
        public Dictionary<string, int> ReportsByStatus { get; set; } = new();
        public Dictionary<string, int> ReportsByType { get; set; } = new();
        public Dictionary<string, int> ReportsByCity { get; set; } = new();
    }
}
