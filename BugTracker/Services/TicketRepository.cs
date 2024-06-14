using Bogus.DataSets;
using BugTracker.Data;
using BugTracker.Model;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Project = BugTracker.Model.Project;

namespace BugTracker.Services
{
    public class TicketRepository(IDbContextFactory<ApplicationDbContext> contextFactory, IServiceProvider serviceProvider) : ITicketRepository
    {
        public async Task AddCommentAsync(TicketComment comment, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            comment.Created = DateTime.Now;

            context.Add(comment);
            await context.SaveChangesAsync();

        }

        public async Task<Ticket> AddTicketAsync(Ticket ticket, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            ticket.Created = DateTimeOffset.Now;

            context.Tickets.Add(ticket);
            await context.SaveChangesAsync();

            return ticket;
        }

        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is not null)
            {
                ticket.IsArchived = true;

                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteCommentAsync(int commentId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketComment? comment = await context.TicketComments.FirstOrDefaultAsync(tc => tc.Id == commentId);

            if (comment is not null)
            {
                context.TicketComments.Remove(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Ticket> tickets = await context.Tickets.Where(t => t.Project!.CompanyId == companyId && t.IsArchived == false).Include(t => t.SubmitterUser).Include(t => t.DeveloperUser).Include(t => t.Project).OrderByDescending(t => t.Created).ToListAsync();

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(int companyId, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            using IServiceScope scope = serviceProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IEnumerable<Ticket> tickets = [];

            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user is null) return tickets;

            bool isPM = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.ProjectManager));

            if (isPM)
            {

                tickets = await context.Tickets
                .Where(t => t.Project.CompanyId == companyId && t.Project.Members.Any(c => c.Id == userId) && t.IsArchived == false || t.SubmitterUserId == userId).Include(t => t.Project)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.SubmitterUser)
                .Include(t => t.DeveloperUser)
                .OrderByDescending(t => t.Created).ToListAsync();

                return tickets;
            }


            bool isDeveloper = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.Developer));
            if (isDeveloper)
            {
                tickets = await context.Tickets.Where(t => t.Project.CompanyId == companyId && t.DeveloperUserId == userId || t.SubmitterUserId == userId)
                                                .Include(t => t.Project)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.Comments)
                                                .Include(t => t.SubmitterUser)
                                                .Include(t => t.DeveloperUser)
                                                .OrderByDescending(t => t.Created)
                                                .ToListAsync();

                return tickets;
            }

            bool isSubmitter = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.Submitter));

            if (isSubmitter)
            {
                tickets = await context.Tickets.Where(t => t.Project.CompanyId == companyId && t.SubmitterUserId == userId)
                                                .Include(t => t.Project)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.Comments)
                                                .Include(t => t.SubmitterUser)
                                                .Include(t => t.DeveloperUser)
                                                .OrderByDescending(t => t.Created)
                                                .ToListAsync();

                return tickets;
            }

            return tickets;
        }

        public async Task<IEnumerable<Ticket>> GetArchivedTickets(int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<Ticket> tickets = await context.Tickets.Where(t => t.Project!.CompanyId == companyId && t.IsArchived == true).Include(t => t.SubmitterUser).Include(t => t.DeveloperUser).Include(t => t.Project).ToListAsync();

            return tickets;
        }

        public async Task<TicketComment?> GetCommentByIdAsync(int commentId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketComment? comment = await context.TicketComments.Include(tc => tc.User).FirstOrDefaultAsync(tc => tc.Id == commentId);

            return comment;
        }

        public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets.Where(t => t.Project!.CompanyId == companyId).Include(t => t.Project).Include(t => t.SubmitterUser).Include(t => t.DeveloperUser).Include(t => t.Attachments).FirstOrDefaultAsync(t => t.Id == ticketId);

            return ticket;
        }

        public async Task<IEnumerable<TicketComment>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            IEnumerable<TicketComment> comments = await context.TicketComments.Where(tc => tc.TicketId == ticketId).Include(tc => tc.User).ToListAsync();

            return comments;
        }

        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            Ticket? ticket = await context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket is not null)
            {
                ticket.IsArchived = false;

                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateCommentAsync(TicketComment comment, int companyId, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            bool shouldUpdate = await context.TicketComments.AnyAsync(tc => tc.Id == comment.Id);

            if (shouldUpdate)
            {
                context.TicketComments.Update(comment);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket, int companyId, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            bool shouldUpdate = await context.Tickets.AnyAsync(t => t.Id == ticket.Id && t.Project!.CompanyId == companyId);

            if (shouldUpdate)
            {
                context.Tickets.Update(ticket);
                await context.SaveChangesAsync();
            }
        }

        public async Task<TicketAttachment> AddTicketAttachment(TicketAttachment attachment, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            // make sure the ticket exists and belongs to this company
            var ticket = await context.Tickets
                .FirstOrDefaultAsync(t => t.Id == attachment.TicketId && t.Project!.CompanyId == companyId);

            // save it if it does
            if (ticket is not null)
            {
                attachment.Created = DateTimeOffset.Now;
                context.TicketAttachments.Add(attachment);
                await context.SaveChangesAsync();

                return attachment;
            }
            else
            {
                throw new ArgumentException("Ticket not found");
            }
        }

        public async Task DeleteTicketAttachment(int attachmentId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            var attachment = await context.TicketAttachments
                .Include(a => a.Upload)
                .FirstOrDefaultAsync(a => a.Id == attachmentId && a.Ticket!.Project!.CompanyId == companyId);

            if (attachment is not null)
            {
                context.Remove(attachment);
                context.Remove(attachment.Upload!);
                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Ticket>> GetUserArchivedTicketsAsync(int companyId, string userId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();
            using IServiceScope scope = serviceProvider.CreateScope();
            UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IEnumerable<Ticket> tickets = [];

            ApplicationUser? user = await userManager.FindByIdAsync(userId);
            if (user is null) return tickets;

            bool isPM = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.ProjectManager));

            if (isPM)
            {

                tickets = await context.Tickets
                .Where(t => t.Project.CompanyId == companyId && t.Project.Members.Any(c => c.Id == userId) && t.IsArchived == true || t.SubmitterUserId == userId).Include(t => t.Project)
                .Include(t => t.Attachments)
                .Include(t => t.Comments)
                .Include(t => t.SubmitterUser)
                .Include(t => t.DeveloperUser)
                .OrderByDescending(t => t.Created).ToListAsync();

                return tickets;
            }


            bool isDeveloper = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.Developer));
            if (isDeveloper)
            {
                tickets = await context.Tickets.Where(t => t.Project.CompanyId == companyId && t.IsArchived == true && t.DeveloperUserId == userId || t.SubmitterUserId == userId)
                                                .Include(t => t.Project)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.Comments)
                                                .Include(t => t.SubmitterUser)
                                                .Include(t => t.DeveloperUser)
                                                .OrderByDescending(t => t.Created)
                                                .ToListAsync();

                return tickets;
            }

            bool isSubmitter = user is not null && await userManager.IsInRoleAsync(user, nameof(Roles.Submitter));

            if (isSubmitter)
            {
                tickets = await context.Tickets.Where(t => t.Project.CompanyId == companyId && t.IsArchived == true && t.SubmitterUserId == userId)
                                                .Include(t => t.Project)
                                                .Include(t => t.Attachments)
                                                .Include(t => t.Comments)
                                                .Include(t => t.SubmitterUser)
                                                .Include(t => t.DeveloperUser)
                                                .OrderByDescending(t => t.Created)
                                                .ToListAsync();

                return tickets;
            }

            return tickets;
        }

        public async Task<TicketAttachment?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId)
        {
            using ApplicationDbContext context = contextFactory.CreateDbContext();

            TicketAttachment? ticketAttachment = await context.TicketAttachments.Where(t => t.Ticket.Project.CompanyId == companyId).FirstOrDefaultAsync(t => t.Id == ticketAttachmentId);

            return ticketAttachment;
        }
    }
}
