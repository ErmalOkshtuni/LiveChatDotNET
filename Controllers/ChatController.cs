using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorixChatService.Data;
using MorixChatService.Models;
using System.Security.Claims;

namespace MorixChatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatServiceDbContext _context;

        public ChatController (ChatServiceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetChat()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var chat = await _context.Chats
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (chat == null)
            {
                return NotFound(new { Message = "Chat not found." });
            }

            return Ok(new { ChatId = chat.Id, UserName = chat.User.UserName });
        }

        [HttpGet("{chatId}/messages")]
        public async Task<IActionResult> GetMessages(int chatId, int pageNumber = 1)
        {
            const int pageSize = 50; 

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntity = await _context.Users.FindAsync(userId) as User;

            if (userEntity == null)
            {
                return Unauthorized();
            }

            var user = userEntity;

            if (user.UserType != UserType.User)
            {
                return Forbid();
            }

            var chat = await _context.Chats
                .Include(c => c.Messages)
                .SingleOrDefaultAsync(c => c.UserId == userId && c.Id == chatId);

            if (chat == null)
            {
                return NotFound("No chat found");
            }

            var totalMessages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .CountAsync();

            var messages = await _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var paginationMetadata = new
            {
                totalCount = totalMessages,
                pageSize = pageSize,
                currentPage = pageNumber,
                totalPages = (int)Math.Ceiling(totalMessages / (double)pageSize)
            };

            return Ok(new { metadata = paginationMetadata, messages = messages });
        }


    }
}
