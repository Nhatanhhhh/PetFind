using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;
using PetStore.Models.Entities;

namespace PetStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly IUserDAO _userDAO;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(IUserDAO userDAO, RoleManager<IdentityRole> roleManager)
        {
            _userDAO = userDAO;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _userDAO.GetAllRolesAsync();
            return View(roles);
        }

        public async Task<IActionResult> Details(int id)
        {
            var role = await _userDAO.GetRoleByIdAsync(id);
            if (role == null || role.Name == "Admin")
                return NotFound();
            return View(role);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(role);
                return RedirectToAction(nameof(Index));
            }
            return View(role);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var role = await _userDAO.GetRoleByIdAsync(id);
            if (role == null || role.Name == "Admin")
                return NotFound();

            var roleDto = new RoleDTO
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
            };

            return View(roleDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RoleDTO roleDto)
        {
            if (id != roleDto.Id)
                return NotFound();

            var existingRole = await _userDAO.GetRoleByIdAsync(id);
            if (existingRole == null || existingRole.Name == "Admin")
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (await _userDAO.RoleExistsByNameAsync(roleDto.Name) && roleDto.Id != id)
                    {
                        ModelState.AddModelError("Name", "Role name already exists.");
                        return View(roleDto);
                    }

                    await _userDAO.UpdateRoleAsync(roleDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _userDAO.RoleExistsByIdAsync(roleDto.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(roleDto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var role = await _userDAO.GetRoleByIdAsync(id);
            if (role == null || role.Name == "Admin")
                return NotFound();
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AssignRole(int userId, string role)
        {
            // Implement role assignment logic in IUserDAO if needed
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RemoveRole(int userId, string role)
        {
            // Implement role removal logic in IUserDAO if needed
            return RedirectToAction(nameof(Index));
        }
    }
}
