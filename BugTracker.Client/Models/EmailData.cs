using System.ComponentModel.DataAnnotations;

namespace BugTracker.Client.Models
{
    public class EmailData
    {
        [Required]
        public string? Recipients { get; set; }

        [Required]
        [Length(5, 160, ErrorMessage = "The {0} must be between {1} and {2} characters")]
        public string? Subject { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Please enter a message at least {1} characters long")]
        public string? Message { get; set; }
    }
}
