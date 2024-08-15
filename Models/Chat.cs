using System.ComponentModel.DataAnnotations;

namespace MorixChatService.Models
{
    public class Chat
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public User? User { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }
}
