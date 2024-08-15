using System.ComponentModel.DataAnnotations;
using MorixChatService.Models;

namespace MorixChatService.DTO
{
    public class ChatDto
    {
        public int Id { get; set; }
        public ICollection<Message> Messages { get; set; }
        public string UserName { get; set; }
    }
}