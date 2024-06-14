using SawyersCodeBlog.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Client.Models
{
    public class CompanyDTO
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ImageURL { get; set; } = ImageHelper.DefaultCategoryImage;
        public virtual ICollection<ProjectDTO> Projects { get; set; } = new HashSet<ProjectDTO>();
        public virtual ICollection<UserDTO> Members { get; set; } = new HashSet<UserDTO>();
        public virtual ICollection<InviteDTO> Invites { get; set; } = new HashSet<InviteDTO>();
    }
}
