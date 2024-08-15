using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MorixChatService.Data;
using System.Security.Claims;
using MorixChatService.Models;

namespace MorixChatService.Hubs
{
    public class ChatHub(ChatServiceDbContext dbContext) : Hub
    {
        private readonly ChatServiceDbContext _dbContext = dbContext;

        public async Task SendMessage(int chatId, string content)
        {
            var userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FindAsync(userId) as User;

            if (user == null)
            {
                throw new HubException("User not found");
            }

            Chat chat = null;

            if (chatId > 0)
            {
                chat = await _dbContext.Chats.Include(c => c.User).SingleOrDefaultAsync(c => c.Id == chatId);

                if (chat == null)
                {
                    throw new HubException("Chat not found");
                }

                if (user.UserType != UserType.Company && chat.UserId != userId)
                {
                    throw new HubException("Access denied");
                }
            }
            else
            {
                chat = await _dbContext.Chats.SingleOrDefaultAsync(c => c.UserId == userId);

                if (chat == null)
                {
                    chat = new Chat
                    {
                        UserId = userId,
                        User = user,
                    };

                    _dbContext.Chats.Add(chat);
                    await _dbContext.SaveChangesAsync();
                }
            }

            var message = new Message
            {
                ChatId = chat.Id,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            await Clients.Group(chat.Id.ToString()).SendAsync("ReceiveMessage", new
            {
                User = user.UserName,
                Content = message.Content,
                Timestamp = message.CreatedAt
            });
        }


        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FindAsync(userId) as User;

            if (user == null)
            {
                throw new HubException("User not found");
            }

            if (user.UserType == UserType.Company)
            {
                var allChats = await _dbContext.Chats.ToListAsync();
                foreach (var chat in allChats)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
                }
            }
            else
            {
                var chat = await _dbContext.Chats.SingleOrDefaultAsync(c => c.UserId == userId);
                if (chat != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, chat.Id.ToString());
                }
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _dbContext.Users.FindAsync(userId) as User;

            if (user == null)
            {
                throw new HubException("User not found");
            }

            if (user.UserType == UserType.Company)
            {
                var allChats = await _dbContext.Chats.ToListAsync();
                foreach (var chat in allChats)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
                }
            }
            else
            {
                var chat = await _dbContext.Chats.SingleOrDefaultAsync(c => c.UserId == userId);
                if (chat != null)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, chat.Id.ToString());
                }
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
