using System.ComponentModel.DataAnnotations;

namespace BugTracker.Client.Models
{
    public class TicketDTO
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
        public virtual ProjectDTO? Project { get; set; }
        [Required]
        public string? SubmitterUserId { get; set; }
        public virtual UserDTO? SubmitterUser { get; set; }
        public string? DeveloperUserId { get; set; }
        public virtual UserDTO? DeveloperUser { get; set; }
        public virtual ICollection<TicketCommentDTO> Comments { get; set; } = new HashSet<TicketCommentDTO>();
        public virtual ICollection<TicketAttachmentDTO> Attachments { get; set; } = new HashSet<TicketAttachmentDTO>();
    }
}
