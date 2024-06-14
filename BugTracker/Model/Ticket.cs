using BugTracker.Client.Models;
using BugTracker.Data;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Model
{
    public class Ticket
    {
        private DateTimeOffset _created;
        private DateTimeOffset? _updated;

        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public DateTimeOffset? Updated
        {
            get => _updated;
            set => _updated = value?.ToUniversalTime();
        }

        public bool IsArchived { get; set; }
        public bool IsArchivedByProject { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public int ProjectId { get; set; }
        public virtual Project? Project { get; set; }
        [Required]
        public string? SubmitterUserId { get; set; }
        public virtual ApplicationUser? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public virtual ApplicationUser? DeveloperUser { get; set; }
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> Attachments { get; set; } = new HashSet<TicketAttachment>();

    }

    public static class TicketExtensions
    {
        public static TicketDTO ToDTO(this Ticket ticket)
        {
            TicketDTO dto = new TicketDTO()
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Created = ticket.Created,
                Updated = ticket.Updated,
                IsArchived = ticket.IsArchived,
                IsArchivedByProject = ticket.IsArchivedByProject,
                Priority = ticket.Priority,
                Type = ticket.Type,
                Status = ticket.Status,
                DeveloperUserId = ticket.DeveloperUserId,
                SubmitterUserId = ticket.SubmitterUserId,
                SubmitterUser = ticket.SubmitterUser?.ToDTO(),
                DeveloperUser = ticket.DeveloperUser?.ToDTO(),
                ProjectId = ticket.ProjectId,

            };

            if (ticket.Project is not null)
            {
                ticket.Project.Tickets = [];
                dto.Project = ticket.Project.ToDTO();
            }

            foreach (TicketComment comment in ticket.Comments)
            {
                TicketCommentDTO ticketCommentDTO = comment.ToDTO();
                dto.Comments.Add(ticketCommentDTO);
            }

            foreach (TicketAttachment attachment in ticket.Attachments)
            {
                TicketAttachmentDTO ticketAttachmentDTO = attachment.ToDTO();
                dto.Attachments.Add(ticketAttachmentDTO);
            }

            return dto;
        }
    }
}
