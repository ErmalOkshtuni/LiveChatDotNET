using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MorixChatService.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        public UserType UserType { get; set; }

        public Chat? Chat { get; set; }

        public ICollection<Message>? Messages { get; set; }
    }

    public enum UserType
    {
        User,
        Company
    }
}
