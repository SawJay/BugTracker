using BugTracker.Client.Models;
using System.Net.Sockets;

namespace BugTracker.Client.Services.Interfaces
{
    public interface ITicketDTOService
    {
        Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId);
        Task<IEnumerable<TicketDTO>> GetArchivedTickets(int companyId);
        Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId);
        Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId);
        Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId);
        Task ArchiveTicketAsync(int ticketId, int companyId);
        Task RestoreTicketAsync(int ticketId, int companyId);

        // Comments

        Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId);

        Task<TicketCommentDTO?> GetCommentByIdAsync(int commentId, int companyId);

        Task AddCommentAsync(TicketCommentDTO comment, int companyId);

        Task DeleteCommentAsync(int commentId, int companyId);

        Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId);

        // Attachments

        Task<TicketAttachmentDTO> AddTicketAttachment(TicketAttachmentDTO attachment, byte[] uploadData, string contentType, int companyId);
        Task DeleteTicketAttachment(int attachmentId, int companyId);

        Task<IEnumerable<TicketDTO>> GetUserTicketsAsync(int companyId, string userId);
        Task<IEnumerable<TicketDTO>> GetUserArchivedTicketsAsync(int companyId, string userId);
        Task<TicketAttachmentDTO?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId);
    }
}
