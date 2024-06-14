using BugTracker.Client.Models;
using BugTracker.Helpers;
using BugTracker.Model;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get => $"{FirstName} {LastName}"; }

        public Guid? ProfilePictureId { get; set; }

        public virtual FileUpload? ProfilePicture { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
    }

    public static class ApplicationUserExtensions
    {
        public static UserDTO ToDTO(this ApplicationUser user)
        {
            UserDTO dto = new()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureId.HasValue ? $"api/uploads/{user.ProfilePictureId}" : UploadHelper.DefaultProfilePicture,
                Email = user.Email,
            };

            return dto;
        }
    }

}
