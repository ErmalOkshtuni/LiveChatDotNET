using System.ComponentModel.DataAnnotations;

namespace MorixChatService.Models
{
    public class Message
    {
        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public int? ChatId { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public Chat? Chat { get; set; }

        public User? User { get; set; }
    }
}
