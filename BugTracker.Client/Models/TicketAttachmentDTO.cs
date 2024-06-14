using SawyersCodeBlog.Helpers;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace BugTracker.Client.Models
{
    public class TicketAttachmentDTO
    {
        private DateTimeOffset _created;

        public int Id { get; set; }
        [Required]
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }

        public string? AttachmentUrl { get; set; }
        [Required]
        public string? UserId { get; set; }
        public virtual UserDTO? User { get; set; }
        public int TicketId { get; set; }
    }
}
