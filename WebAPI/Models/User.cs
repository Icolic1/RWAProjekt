using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        public string Username { get; set; } = null!;

        public string PasswordHash { get; set; } = null!; // ili pw
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!; // e.g., "Admin" or "User"

        // navigacijska svojstva foreign key
        public ICollection<UserHeritageComment> UserHeritageComments { get; set; }= new List<UserHeritageComment>();
    }
}
