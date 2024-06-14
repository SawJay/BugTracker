using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Model;
using BugTracker.Services.Interfaces;
using Microsoft.CodeAnalysis;
using System.Net.Sockets;

namespace BugTracker.Services
{
    public class TicketDTOService : ITicketDTOService
    {
        private readonly ITicketRepository _repository;

        public TicketDTOService(ITicketRepository repository)
        {
            _repository = repository;
        }

        public async Task AddCommentAsync(TicketCommentDTO comment, int companyId)
        {
            TicketComment newComment = new TicketComment()
            {
                Content = comment.Content,
                TicketId = comment.TicketId,
                Created = DateTimeOffset.Now,
                UserId = comment.UserId,
            };

            await _repository.AddCommentAsync(newComment, companyId);
        }

        public async Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId)
        {
            Ticket newTicket = new Ticket()
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
                Status = TicketStatus.New,
                ProjectId = ticket.ProjectId,
                SubmitterUserId = ticket.SubmitterUserId,
                DeveloperUserId = ticket.DeveloperUserId,

            };

            newTicket = await _repository.AddTicketAsync(newTicket, companyId);

            return newTicket.ToDTO();
        }

        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            await _repository.ArchiveTicketAsync(ticketId, companyId);
        }

        public async Task DeleteCommentAsync(int commentId, int companyId)
        {
            await _repository.DeleteCommentAsync(commentId, companyId);
        }

        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId)
        {
            IEnumerable<Ticket> tickets = await _repository.GetAllTicketsAsync(companyId);

            IEnumerable<TicketDTO> ticketDTOs = tickets.Select(p => p.ToDTO());

            return ticketDTOs;
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTickets(int companyId)
        {
            IEnumerable<Ticket> tickets = await _repository.GetArchivedTickets(companyId);

            IEnumerable<TicketDTO> ticketDTOs = tickets.Select(p => p.ToDTO());

            return ticketDTOs;
        }

        public async Task<TicketCommentDTO?> GetCommentByIdAsync(int commentId, int companyId)
        {
            TicketComment? comment = await _repository.GetCommentByIdAsync(commentId, companyId);
            return comment?.ToDTO();
        }

        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            Ticket? ticket = await _repository.GetTicketByIdAsync(ticketId, companyId);
            return ticket?.ToDTO();
        }

        public async Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            IEnumerable<TicketComment> comments = await _repository.GetTicketCommentsAsync(ticketId, companyId);

            IEnumerable<TicketCommentDTO> commentDTOs = comments.Select(c => c.ToDTO());

            return commentDTOs;
        }

        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            await _repository.RestoreTicketAsync(ticketId, companyId);
        }

        public async Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId)
        {
            TicketComment? commentToUpdate = await _repository.GetCommentByIdAsync(comment.Id, companyId);

            if (commentToUpdate is not null)
            {
                commentToUpdate.Content = comment.Content;
                commentToUpdate.TicketId = comment.TicketId;

                await _repository.UpdateCommentAsync(commentToUpdate, companyId, userId);
            }
        }

        public async Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId)
        {
            Ticket? ticketToUpdate = await _repository.GetTicketByIdAsync(ticket.Id, companyId);

            if (ticketToUpdate is not null)
            {
                ticketToUpdate.Title = ticket.Title;
                ticketToUpdate.Description = ticket.Description;
                ticketToUpdate.Created = ticket.Created;
                ticketToUpdate.Updated = ticket.Updated;
                ticketToUpdate.IsArchived = ticket.IsArchived;
                ticketToUpdate.IsArchivedByProject = ticket.IsArchivedByProject;
                ticketToUpdate.Priority = ticket.Priority;
                ticketToUpdate.Type = ticket.Type;
                ticketToUpdate.Status = ticket.Status;
                ticketToUpdate.ProjectId = ticket.ProjectId;
                ticketToUpdate.SubmitterUserId = ticket.SubmitterUserId;
                ticketToUpdate.DeveloperUserId = ticket.DeveloperUserId;

                ticketToUpdate.DeveloperUser = null;

                await _repository.UpdateTicketAsync(ticketToUpdate, companyId, userId);
            }
        }

        public async Task<TicketAttachmentDTO> AddTicketAttachment(TicketAttachmentDTO attachment, byte[] uploadData, string contentType, int companyId)
        {
            FileUpload file = new()
            {
                Type = contentType,
                Data = uploadData,
            };

            TicketAttachment dbAttachment = new()
            {
                TicketId = attachment.TicketId,
                Description = attachment.Description,
                FileName = attachment.FileName,
                Upload = file,
                Created = DateTimeOffset.Now,
                UserId = attachment.UserId
            };

            dbAttachment = await _repository.AddTicketAttachment(dbAttachment, companyId);

            return dbAttachment.ToDTO();
        }

        public async Task DeleteTicketAttachment(int attachmentId, int companyId)
        {
            await _repository.DeleteTicketAttachment(attachmentId, companyId);
        }

        public async Task<IEnumerable<TicketDTO>> GetUserTicketsAsync(int companyId, string userId)
        {
            IEnumerable<Ticket> userTickets = await _repository.GetUserTicketsAsync(companyId, userId);

            IEnumerable<TicketDTO> userTicketDTOs = userTickets.Select(p => p.ToDTO());

            return userTicketDTOs;
        }

        public async Task<IEnumerable<TicketDTO>> GetUserArchivedTicketsAsync(int companyId, string userId)
        {
            IEnumerable<Ticket> userTickets = await _repository.GetUserArchivedTicketsAsync(companyId, userId);

            IEnumerable<TicketDTO> userTicketDTOs = userTickets.Select(p => p.ToDTO());

            return userTicketDTOs;
        }

        public async Task<TicketAttachmentDTO?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId)
        {
            TicketAttachment? ticketAttachment = await _repository.GetTicketAttachmentByIdAsync(ticketAttachmentId, companyId);
            return ticketAttachment?.ToDTO();
        }
    }
}
