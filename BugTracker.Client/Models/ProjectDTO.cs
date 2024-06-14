using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace BugTracker.Client.Models
{
    public class ProjectDTO
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
        public virtual ICollection<UserDTO> Members { get; set; } = new HashSet<UserDTO>();
        public virtual ICollection<TicketDTO> Tickets { get; set; } = new HashSet<TicketDTO>();
    }
}
