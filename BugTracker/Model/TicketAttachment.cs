using BugTracker.Client.Models;
using BugTracker.Data;
using BugTracker.Helpers;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Model
{
    public class TicketAttachment
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

        public Guid? UploadId { get; set; }

        public virtual FileUpload? Upload { get; set; }
        [Required]
        public string? UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        [Required] 
        public int TicketId { get; set; }
        public virtual Ticket? Ticket { get; set; }

    }

    public static class TicketAttachmentExtensions
    {
        public static TicketAttachmentDTO ToDTO(this TicketAttachment ticketAttachment)
        {
            TicketAttachmentDTO dto = new TicketAttachmentDTO()
            {
                Id = ticketAttachment.Id,
                FileName = ticketAttachment.FileName,
                Description = ticketAttachment.Description,
                Created = ticketAttachment.Created,
                User = ticketAttachment.User?.ToDTO(),
                UserId = ticketAttachment.UserId,
                TicketId = ticketAttachment.TicketId,
                AttachmentUrl = ticketAttachment.UploadId.HasValue ? $"api/uploads/{ticketAttachment.UploadId}" : UploadHelper.DefaultProfilePicture,
            };

            return dto;
        }
    }
}
