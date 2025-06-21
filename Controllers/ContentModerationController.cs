using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;

namespace PetStore.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class ContentModerationController : Controller
    {
        private readonly IContentModerationDAO _moderationDAO;
        private readonly IUserDAO _userDAO;

        public ContentModerationController(IContentModerationDAO moderationDAO, IUserDAO userDAO)
        {
            _moderationDAO = moderationDAO;
            _userDAO = userDAO;
        }

        public async Task<IActionResult> Index()
        {
            var moderations = await _moderationDAO.GetAllModerationsAsync();
            return View(moderations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var moderation = await _moderationDAO.GetModerationByIdAsync(id);
            if (moderation == null)
                return NotFound();
            return View(moderation);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContentModerationCreateDTO moderationDto)
        {
            if (ModelState.IsValid)
            {
                await _moderationDAO.CreateModerationAsync(moderationDto);
                return RedirectToAction(nameof(Index));
            }
            return View(moderationDto);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var moderation = await _moderationDAO.GetModerationByIdAsync(id);
            if (moderation == null)
                return NotFound();

            var updateDto = new ContentModerationUpdateDTO
            {
                Id = moderation.Id,
                Status = moderation.Status,
                RejectionReason = moderation.RejectionReason,
            };

            return View(updateDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContentModerationUpdateDTO moderationDto)
        {
            if (id != moderationDto.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _moderationDAO.UpdateModerationAsync(id, moderationDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    if (!await _moderationDAO.ModerationExistsAsync(id))
                        return NotFound();
                    throw;
                }
            }
            return View(moderationDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var moderation = await _moderationDAO.GetModerationByIdAsync(id);
            if (moderation == null)
                return NotFound();
            return View(moderation);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _moderationDAO.DeleteModerationAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Moderator(int id)
        {
            var moderations = await _moderationDAO.GetModerationsByModeratorIdAsync(id);
            return View(moderations);
        }
    }
}
