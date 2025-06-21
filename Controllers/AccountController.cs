using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;
using PetStore.Services;

namespace PetStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserDAO _userDAO;
        private readonly IFileService _fileService;

        public AccountController(IUserDAO userDAO, IFileService fileService)
        {
            _userDAO = userDAO;
            _fileService = fileService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _userDAO.IsEmailUniqueAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                var user = await _userDAO.CreateUserAsync(model);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email),
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginRequestDTO { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDTO model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = await _userDAO.GetByEmailWithRolesAsync(model.Email);

                if (
                    user != null
                    && user.PasswordHash == HashPassword(model.Password)
                    && user.IsActive
                )
                {
                    await _userDAO.UpdateLastLoginAsync(user.Id);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                    };

                    if (!string.IsNullOrEmpty(user.AvatarUrl))
                    {
                        claims.Add(new Claim("AvatarUrl", user.AvatarUrl));
                    }

                    var roles =
                        user.UserRoles?.Select(ur => ur.Role.Name) ?? Enumerable.Empty<string>();
                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(
                            new ClaimsIdentity(
                                claims,
                                CookieAuthenticationDefaults.AuthenticationScheme
                            )
                        ),
                        authProperties
                    );

                    return RedirectToLocal(returnUrl);
                }

                ModelState.AddModelError(
                    string.Empty,
                    "Email hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa"
                );
            }
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> LogoutPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            var user = await _userDAO.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(UserProfileUpdateDTO model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                if (userId == 0)
                    return Unauthorized();

                try
                {
                    // Process image input (file upload or URL)
                    var newImageUrl = await _fileService.ProcessImageInputAsync(
                        model.AvatarFile,
                        model.AvatarUrl,
                        "avatars"
                    );

                    var userToUpdate = await _userDAO.GetUserByIdAsync(userId);

                    var updateDto = new UserUpdateDTO
                    {
                        Id = userId,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        // Only update the avatar if a new one was provided, otherwise keep the old one
                        AvatarUrl = newImageUrl ?? userToUpdate.AvatarUrl,
                        IsActive = true,
                        LastLoginAt = DateTime.UtcNow,
                    };

                    await _userDAO.UpdateUserAsync(userId, updateDto);

                    // Re-issue the authentication cookie to reflect the changes (like new AvatarUrl)
                    var updatedUser = await _userDAO.GetByEmailWithRolesAsync(userToUpdate.Email);
                    if (updatedUser != null)
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, updatedUser.Id.ToString()),
                            new Claim(ClaimTypes.Name, updatedUser.Email),
                            new Claim(ClaimTypes.Email, updatedUser.Email),
                        };

                        if (!string.IsNullOrEmpty(updatedUser.AvatarUrl))
                        {
                            claims.Add(new Claim("AvatarUrl", updatedUser.AvatarUrl));
                        }

                        var roles =
                            updatedUser.UserRoles?.Select(ur => ur.Role.Name)
                            ?? Enumerable.Empty<string>();
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role));
                        }

                        // Get current authentication properties to persist the login
                        var authResult = await HttpContext.AuthenticateAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme
                        );

                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(
                                new ClaimsIdentity(
                                    claims,
                                    CookieAuthenticationDefaults.AuthenticationScheme
                                )
                            ),
                            authResult?.Properties // Re-use the existing properties (like IsPersistent)
                        );
                    }

                    TempData["Success"] = "Profile updated successfully.";
                    return RedirectToAction(nameof(Profile));
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("AvatarFile", ex.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while updating the profile.");
                }
            }
            // If we got this far, something failed, redisplay form
            // Ensure you have the user's current info to repopulate the form correctly
            var currentUser = await _userDAO.GetUserByIdAsync(
                int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0")
            );
            var editModel = new UserProfileUpdateDTO
            {
                FirstName = model.FirstName ?? currentUser.FirstName,
                LastName = model.LastName ?? currentUser.LastName,
                PhoneNumber = model.PhoneNumber ?? currentUser.PhoneNumber,
                Address = model.Address ?? currentUser.Address,
                AvatarUrl = currentUser.AvatarUrl, // Show the existing avatar
            };
            return View("EditProfile", editModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            var user = await _userDAO.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            var editModel = new UserProfileUpdateDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                AvatarUrl = user.AvatarUrl,
            };

            return View(editModel);
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}
