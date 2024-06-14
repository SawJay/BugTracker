using BugTracker.Client.Models;
using BugTracker.Data;
using BugTracker.Helpers;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Model
{
    public class Company
    {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }
        public Guid? ImageId { get; set; }

        public virtual FileUpload? Image { get; set; }

        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();

        public virtual ICollection<ApplicationUser> Members { get; set; } = new HashSet<ApplicationUser>();

        public virtual ICollection<Invite> Invites { get; set; } = new HashSet<Invite>();
    }

    public static class CompanyExtensions
    {
        public static CompanyDTO ToDTO(this Company company)
        {
            CompanyDTO dto = new()
            {
                Id = company.Id,
                Name = company.Name,
                Description = company.Description,
                ImageURL = company.ImageId.HasValue ? $"api/uploads/{company.ImageId}" : UploadHelper.DefaultProfilePicture,
            };

            foreach (Project project in company.Projects)
            {
                ProjectDTO projectDTO = project.ToDTO();
                dto.Projects.Add(projectDTO);
            }

            foreach (Invite invite in company.Invites)
            {
                InviteDTO inviteDTO = invite.ToDTO();
                dto.Invites.Add(inviteDTO);
            }

            foreach (ApplicationUser user in company.Members)
            {
                UserDTO userDTO = user.ToDTO();
                dto.Members.Add(userDTO);
            }

            return dto;

        }
    }
}
