using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace BugTracker.Client.Models
{
    public class TicketCommentDTO
    {
        private DateTimeOffset _created;

        public int Id { get; set; }
        [Required]
        public string? Content { get; set; }
        public DateTimeOffset Created
        {
            get => _created;
            set => _created = value.ToUniversalTime();
        }
        public int TicketId { get; set; }
        public virtual UserDTO? User { get; set; }
        [Required]
        public string? UserId { get; set; }
    }
}
