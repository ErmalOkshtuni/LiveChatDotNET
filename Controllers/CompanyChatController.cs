using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MorixChatService.Data;
using MorixChatService.Models;
using System.Security.Claims;
using MorixChatService.DTO;

namespace MorixChatService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyChatController : ControllerBase
    {
        private readonly ChatServiceDbContext _dbContext;

        public CompanyChatController(ChatServiceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("chats")]
        public async Task<IActionResult> GetAllChats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntiy = await _dbContext.Users.FindAsync(userId) as User;

            if(userEntiy == null)
            {
                return Unauthorized();
            }

            var user = userEntiy;

            if (user.UserType != UserType.Company )
            {
                return Forbid();
            }

            var chats = await _dbContext.Chats
                .Include(c => c.User)
                .Select(c => new ChatDto
                {
                    Id = c.Id,
                    Messages = c.Messages,
                    UserName = c.User.UserName
                })
                .ToListAsync();

            return Ok(chats);
        }

        [HttpGet("chats/{chatId}/messages")]
        public async Task<IActionResult> GetMessages(int chatId, int pageNumber = 1)
        {
            const int pageSize = 50; 

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEntity = await _dbContext.Users.FindAsync(userId) as User;

            if (userEntity == null)
            {
                return Unauthorized();
            }

            var user = userEntity;

            if (user.UserType != UserType.Company)
            {
                return Forbid();
            }

            var chat = await _dbContext.Chats.SingleOrDefaultAsync(c => c.Id == chatId);

            if (chat == null)
            {
                return NotFound("Chat not found");
            }

            var totalMessages = await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .CountAsync();

            var messages = await _dbContext.Messages
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
