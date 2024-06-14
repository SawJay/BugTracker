using System.ComponentModel.DataAnnotations;

namespace BugTracker.Client.Models
{
    public class UserDTO
    {
        [Required]
        public string? Id { get; set; }
        [Required]
        public  string? Email { get; set; }
        [Required]
        public  string? FirstName { get; set; }
        [Required]
        public  string? LastName { get; set; }
        [Required]
        public  string? ProfilePictureUrl { get; set; }
        public string FullName => $"{FirstName} {LastName}";
        public string? Role { get; set; }
    }
}
