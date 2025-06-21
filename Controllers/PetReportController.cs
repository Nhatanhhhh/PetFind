using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;
using PetStore.Models.Enums;
using PetStore.Services;

namespace PetStore.Controllers
{
    [Authorize]
    public class PetReportController : Controller
    {
        private readonly IPetReportDAO _petReportDAO;
        private readonly IFileService _fileService;

        public PetReportController(IPetReportDAO petReportDAO, IFileService fileService)
        {
            _petReportDAO = petReportDAO;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var reports = await _petReportDAO.GetAllReportsAsync();
            return View(reports);
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _petReportDAO.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();
            return View(report);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PetReportCreateDTO reportDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                    if (userId == 0)
                        return Unauthorized();

                    reportDto.UserId = userId;

                    // Process image input (file upload or URL)
                    var imageUrl = await _fileService.ProcessImageInputAsync(
                        reportDto.ImageFile,
                        reportDto.ImageUrl,
                        "pets"
                    );
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        reportDto.ImageUrl = imageUrl;
                    }

                    var result = await _petReportDAO.CreateReportAsync(reportDto);

                    TempData["Success"] =
                        "Báo cáo đã được gửi thành công và đang chờ Admin duyệt. Bạn sẽ nhận được thông báo khi báo cáo được duyệt.";

                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(reportDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var report = await _petReportDAO.GetReportByIdAsync(id);
            if (report == null)
                return NotFound();

            var updateDto = new PetReportUpdateDTO
            {
                Id = report.Id,
                Type = report.Type,
                Species = report.Species,
                Title = report.Title,
                PetName = report.PetName,
                Breed = report.Breed,
                Color = report.Color,
                Description = report.Description,
                District = report.District,
                City = report.City,
                LostOrFoundDate = report.LostOrFoundDate ?? DateTime.MinValue,
                Status = report.Status,
                ContactName = report.ContactName,
                ContactPhone = report.ContactPhone,
                Latitude = report.Latitude,
                Longitude = report.Longitude,
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PetReportUpdateDTO reportDto)
        {
            if (id != reportDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Process image input (file upload or URL)
                    var imageUrl = await _fileService.ProcessImageInputAsync(
                        reportDto.ImageFile,
                        reportDto.ImageUrl,
                        "pets"
                    );
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        reportDto.ImageUrl = imageUrl;
                    }

                    var result = await _petReportDAO.UpdateReportAsync(id, reportDto);
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("ImageFile", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            return View(reportDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _petReportDAO.DeleteReportAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<IActionResult> UpdateStatus(int id, [FromForm] PetReportStatus status)
        {
            try
            {
                await _petReportDAO.UpdateStatusAsync(id, status);
                TempData["Success"] = $"Report status updated to {status} successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating report status: {ex.Message}";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Search(PetReportSearchDTO searchDto)
        {
            var reports = await _petReportDAO.SearchReportsAsync(searchDto);
            return View("Index", reports);
        }

        public async Task<IActionResult> MyReports()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return RedirectToAction("Login", "Account");

            var reports = await _petReportDAO.GetReportsByUserAsync(userId);
            return View(reports);
        }

        private string GenerateSlug(string title)
        {
            return title
                .ToLower()
                .Replace(" ", "-")
                .Replace("đ", "d")
                .Replace("Đ", "D")
                .Replace("á", "a")
                .Replace("à", "a")
                .Replace("ả", "a")
                .Replace("ã", "a")
                .Replace("ạ", "a")
                .Replace("ă", "a")
                .Replace("ắ", "a")
                .Replace("ằ", "a")
                .Replace("ẳ", "a")
                .Replace("ẵ", "a")
                .Replace("ặ", "a")
                .Replace("â", "a")
                .Replace("ấ", "a")
                .Replace("ầ", "a")
                .Replace("ẩ", "a")
                .Replace("ẫ", "a")
                .Replace("ậ", "a")
                .Replace("é", "e")
                .Replace("è", "e")
                .Replace("ẻ", "e")
                .Replace("ẽ", "e")
                .Replace("ẹ", "e")
                .Replace("ê", "e")
                .Replace("ế", "e")
                .Replace("ề", "e")
                .Replace("ể", "e")
                .Replace("ễ", "e")
                .Replace("ệ", "e")
                .Replace("í", "i")
                .Replace("ì", "i")
                .Replace("ỉ", "i")
                .Replace("ĩ", "i")
                .Replace("ị", "i")
                .Replace("ó", "o")
                .Replace("ò", "o")
                .Replace("ỏ", "o")
                .Replace("õ", "o")
                .Replace("ọ", "o")
                .Replace("ô", "o")
                .Replace("ố", "o")
                .Replace("ồ", "o")
                .Replace("ổ", "o")
                .Replace("ỗ", "o")
                .Replace("ộ", "o")
                .Replace("ơ", "o")
                .Replace("ớ", "o")
                .Replace("ờ", "o")
                .Replace("ở", "o")
                .Replace("ỡ", "o")
                .Replace("ợ", "o")
                .Replace("ú", "u")
                .Replace("ù", "u")
                .Replace("ủ", "u")
                .Replace("ũ", "u")
                .Replace("ụ", "u")
                .Replace("ư", "u")
                .Replace("ứ", "u")
                .Replace("ừ", "u")
                .Replace("ử", "u")
                .Replace("ữ", "u")
                .Replace("ự", "u")
                .Replace("ý", "y")
                .Replace("ỳ", "y")
                .Replace("ỷ", "y")
                .Replace("ỹ", "y")
                .Replace("ỵ", "y");
        }
    }
}
