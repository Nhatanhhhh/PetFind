using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;
using PetStore.Models.Enums;
using PetStore.Services;

namespace PetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IPetReportDAO _petReportDAO;
        private readonly IContentModerationDAO _contentModerationDAO;
        private readonly IUserDAO _userDAO;
        private readonly IMessageDAO _messageDAO;
        private readonly IFileService _fileService;

        public AdminController(
            IPetReportDAO petReportDAO,
            IContentModerationDAO contentModerationDAO,
            IUserDAO userDAO,
            IMessageDAO messageDAO,
            IFileService fileService
        )
        {
            _petReportDAO = petReportDAO;
            _contentModerationDAO = contentModerationDAO;
            _userDAO = userDAO;
            _messageDAO = messageDAO;
            _fileService = fileService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var allReports = await _petReportDAO.GetAllReportsAsync();
            var allModerations = await _contentModerationDAO.GetAllModerationsAsync();
            var allUsers = await _userDAO.GetAllUsersAsync();
            var conversations = await _messageDAO.GetUserConversationsAsync(1);
            var lostReports = await _petReportDAO.GetReportsByTypeAsync("Lost");
            var foundReports = await _petReportDAO.GetReportsByTypeAsync("Found");
            var pendingReports = await _petReportDAO.GetPendingReportsAsync();

            var dashboardData = new AdminDashboardDTO
            {
                TotalReports = allReports.Count(),
                PendingModerations = allModerations.Count(m =>
                    m.Status == ContentModerationStatus.Pending
                ),
                TotalUsers = allUsers.Count(),
                TotalMessages = conversations.Count(),
                LostReportsCount = lostReports.Count(),
                FoundReportsCount = foundReports.Count(),
                ActiveUsersCount = allUsers.Count(u => u.IsActive),
                InactiveUsersCount = allUsers.Count(u => !u.IsActive),
                PendingReports = pendingReports.ToList(),
                RecentActivities = allModerations.Take(10).ToList(),
                RecentUsers = allUsers.OrderByDescending(u => u.CreatedAt).Take(5).ToList(),
                ReportsByStatus = allReports
                    .GroupBy(r => r.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                ReportsByType = allReports
                    .GroupBy(r => r.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ReportsByCity = allReports
                    .GroupBy(r => r.City)
                    .ToDictionary(g => g.Key, g => g.Count()),
            };

            return View(dashboardData);
        }

        public async Task<IActionResult> Users()
        {
            var users = await _userDAO.GetAllUsersAsync();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUserRole(int userId, string role)
        {
            try
            {
                // Implement role update logic
                TempData["Success"] = $"User role updated to {role} successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating user role: {ex.Message}";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(int userId)
        {
            try
            {
                var user = await _userDAO.GetUserByIdAsync(userId);
                if (user != null)
                {
                    await _userDAO.UpdateUserAsync(
                        userId,
                        new UserUpdateDTO { IsActive = !user.IsActive }
                    );
                    TempData["Success"] =
                        $"User {(user.IsActive ? "deactivated" : "activated")} successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating user status: {ex.Message}";
            }
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userDAO.DeleteUserAsync(userId);
                TempData["Success"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting user: {ex.Message}";
            }
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Reports()
        {
            var reports = await _petReportDAO.GetAllReportsAsync();
            return View(reports);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                await _petReportDAO.DeleteReportAsync(id);
                TempData["Success"] = "Report deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting report: {ex.Message}";
            }
            return RedirectToAction(nameof(Reports));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReportStatus(int id, PetReportStatus status)
        {
            try
            {
                var report = await _petReportDAO.GetReportByIdAsync(id);
                if (report != null)
                {
                    var updateDto = new PetReportUpdateDTO
                    {
                        Id = id,
                        Type = report.Type,
                        Species = report.Species,
                        Title = report.Title,
                        PetName = report.PetName,
                        Breed = report.Breed,
                        Color = report.Color,
                        Description = report.Description,
                        District = report.District,
                        City = report.City,
                        LostOrFoundDate = report.LostOrFoundDate ?? DateTime.UtcNow,
                        Status = status,
                        ContactName = report.ContactName,
                        ContactPhone = report.ContactPhone,
                        ContactEmail = report.ContactEmail,
                        ContactNote = report.ContactNote,
                        IsAISearchEnabled = report.IsAISearchEnabled,
                        Latitude = report.Latitude,
                        Longitude = report.Longitude,
                    };

                    await _petReportDAO.UpdateReportAsync(id, updateDto);
                    TempData["Success"] = $"Report status updated to {status} successfully.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating report status: {ex.Message}";
            }
            return RedirectToAction(nameof(Reports));
        }

        public async Task<IActionResult> Moderations()
        {
            var moderations = await _contentModerationDAO.GetAllModerationsAsync();
            return View(moderations);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveModeration(int id)
        {
            try
            {
                var moderatorId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
                );
                if (moderatorId == 0)
                    return Unauthorized();

                var success = await _contentModerationDAO.ApproveModerationAsync(id, moderatorId);
                if (success)
                {
                    TempData["Success"] = "Report approved successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to approve report.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error approving report: {ex.Message}";
            }
            return RedirectToAction(nameof(Moderations));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectModeration(int id, string reason)
        {
            try
            {
                var moderatorId = int.Parse(
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"
                );
                if (moderatorId == 0)
                    return Unauthorized();

                var success = await _contentModerationDAO.RejectModerationAsync(
                    id,
                    moderatorId,
                    reason
                );
                if (success)
                {
                    TempData["Success"] = "Report rejected successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to reject report.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error rejecting report: {ex.Message}";
            }
            return RedirectToAction(nameof(Moderations));
        }

        public async Task<IActionResult> Statistics()
        {
            // Gọi tuần tự để tránh concurrency issues
            var allReports = await _petReportDAO.GetAllReportsAsync();
            var allModerations = await _contentModerationDAO.GetAllModerationsAsync();
            var allUsers = await _userDAO.GetAllUsersAsync();

            var statistics = new AdminStatisticsDTO
            {
                TotalUsers = allUsers.Count(),
                ActiveUsers = allUsers.Count(u => u.IsActive),
                TotalReports = allReports.Count(),
                PendingReports = allModerations.Count(m =>
                    m.Status == ContentModerationStatus.Pending
                ),
                ApprovedReports = allModerations.Count(m =>
                    m.Status == ContentModerationStatus.Approved
                ),
                RejectedReports = allModerations.Count(m =>
                    m.Status == ContentModerationStatus.Rejected
                ),
                TotalMessages = 0, // Will be implemented when message statistics are available
                ReportsByStatus = allReports
                    .GroupBy(r => r.Status.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                ReportsByType = allReports
                    .GroupBy(r => r.Type)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ReportsByCity = allReports
                    .GroupBy(r => r.City)
                    .ToDictionary(g => g.Key, g => g.Count()),
                ReportsByMonth = allReports
                    .GroupBy(r => r.CreatedAt.ToString("yyyy-MM"))
                    .ToDictionary(g => g.Key, g => g.Count()),
                UsersByMonth = allUsers
                    .GroupBy(u => u.CreatedAt.ToString("yyyy-MM"))
                    .ToDictionary(g => g.Key, g => g.Count()),
            };

            return View(statistics);
        }

        public IActionResult SystemSettings()
        {
            // Admin can manage system settings
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateSystemSettings()
        {
            // Implement system settings update
            TempData["Success"] = "System settings updated successfully.";
            return RedirectToAction(nameof(SystemSettings));
        }
    }
}
