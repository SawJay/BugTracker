using System.ComponentModel.DataAnnotations;

namespace BugTracker.Client.Models
{
    public class InviteDTO
    {
        private DateTimeOffset _inviteDate;

        private DateTimeOffset? _joinDate;

        public int Id { get; set; }

        public DateTimeOffset InviteDate
        {
            get => _inviteDate;
            set => _inviteDate = value.ToUniversalTime();
        }

        public DateTimeOffset? JoinDate
        {
            get => _joinDate?.ToLocalTime();
            set => _joinDate = value?.ToUniversalTime();
        }

        [Required]
        public string? InviteeEmail { get; set; }
        [Required]
        public string? InviteeFirstName { get; set; }
        [Required]
        public string? InviteeLastName { get; set; }
        public string? Message { get; set; }
        public bool IsValid { get; set; }
        public int ProjectId { get; set; }
        public virtual ProjectDTO? Project { get; set; }
        [Required]
        public string? InviterId { get; set; }
        public virtual UserDTO? Invitor { get; set; }
        public string? InviteeId { get; set; }
        public virtual UserDTO? Invitee { get; set; }
    }
}
