using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        
        public string PasswordHash { get; set; } // ili Password ako koristite običan string
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; }
        public string Role { get; set; } // e.g., "Admin" or "User"

        // Navigation properties
        public ICollection<UserHeritageComment> UserHeritageComments { get; set; }
    }
}
