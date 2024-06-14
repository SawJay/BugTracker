using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net.Sockets;
using BugTracker.Client.Models;
using BugTracker.Data;

namespace BugTracker.Model
{
    public class Project
    {
        private DateTimeOffset _created;
        private DateTimeOffset _startDate;
        private DateTimeOffset _endDate;

        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public DateTimeOffset Created 
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        [Required]
        public DateTimeOffset StartDate
        {
            get => _startDate;
            set => _startDate = value.ToUniversalTime();
        }

        [Required]
        public DateTimeOffset EndDate
        {
            get => _endDate;
            set => _endDate = value.ToUniversalTime();
        }

        [Required]
        public ProjectPriority Priority { get; set; }
        public bool IsArchived { get; set; }
        public int CompanyId { get; set; }
        public virtual Company? Company { get; set; }
        public virtual ICollection<ApplicationUser> Members { get; set; } = new HashSet<ApplicationUser>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();

    }

    public static class ProjectExtensions
    {
        public static ProjectDTO ToDTO(this Project project)
        {
            ProjectDTO dto = new()
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Created = project.Created,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
                IsArchived = project.IsArchived,
            };

            foreach(ApplicationUser user in project.Members)
            {
                user.Projects.Clear();

                UserDTO userDTO = user.ToDTO();
                dto.Members.Add(userDTO);
            }

            foreach(Ticket ticket in project.Tickets)
            {
                TicketDTO ticketDTO = ticket.ToDTO();
                dto.Tickets.Add(ticketDTO);
            }

            return dto;
        }
    }
}
