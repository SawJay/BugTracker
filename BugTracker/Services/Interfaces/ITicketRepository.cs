using BugTracker.Data;
using BugTracker.Model;

namespace BugTracker.Services.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId);
        Task<IEnumerable<Ticket>> GetArchivedTickets(int companyId);
        Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId);
        Task<Ticket> AddTicketAsync(Ticket ticket, int companyId);
        // you will not need userId at this point, but put it in the method anyways for future authorization features O references
        Task UpdateTicketAsync(Ticket ticket, int companyId, string userId);
        Task ArchiveTicketAsync(int ticketId, int companyId);
        Task RestoreTicketAsync(int ticketId, int companyId);

        // Comments 

        Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(int ticketId, int companyId);

        Task<TicketComment?> GetCommentByIdAsync(int commentId, int companyId);

        Task AddCommentAsync(TicketComment comment, int companyId);

        Task DeleteCommentAsync(int commentId, int companyId);

        Task UpdateCommentAsync(TicketComment comment, int companyId, string userId);

        // Attachments

        Task<TicketAttachment> AddTicketAttachment(TicketAttachment attachment, int companyId);
        Task DeleteTicketAttachment(int attachmentId, int companyId);

        Task<IEnumerable<Ticket>> GetUserTicketsAsync(int companyId, string userId);
        Task<IEnumerable<Ticket>> GetUserArchivedTicketsAsync(int companyId, string userId);
        Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId);

    }
}
