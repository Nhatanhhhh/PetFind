using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetStore.DAO.Interfaces;
using PetStore.Models.DTOs;

namespace PetStore.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageDAO _messageDAO;
        private readonly IUserDAO _userDAO;

        public MessageController(IMessageDAO messageDAO, IUserDAO userDAO)
        {
            _messageDAO = messageDAO;
            _userDAO = userDAO;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
                return Unauthorized();

            var conversations = await _messageDAO.GetUserConversationsAsync(userId);
            return View(conversations);
        }

        public async Task<IActionResult> Chat(int userId)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (currentUserId == 0)
                return Unauthorized();

            var messages = await _messageDAO.GetMessagesBetweenUsersAsync(currentUserId, userId);
            var otherUser = await _userDAO.GetUserByIdAsync(userId);

            ViewBag.OtherUser = otherUser;
            ViewBag.CurrentUserId = currentUserId;
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(
            int receiverId,
            string content,
            int? petReportId = null
        )
        {
            if (string.IsNullOrWhiteSpace(content))
                return BadRequest("Message content cannot be empty");

            var senderId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (senderId == 0)
                return Unauthorized();

            var messageDto = new MessageCreateDTO
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                PetReportId = petReportId,
            };

            await _messageDAO.CreateMessageAsync(messageDto);
            return RedirectToAction(nameof(Chat), new { userId = receiverId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(int messageId)
        {
            var success = await _messageDAO.MarkAsReadAsync(messageId);
            if (!success)
                return NotFound();
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _messageDAO.DeleteMessageAsync(id);
            if (!success)
                return NotFound();
            return RedirectToAction(nameof(Index));
        }
    }
}
