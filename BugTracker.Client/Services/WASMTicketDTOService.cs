using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace BugTracker.Client.Services
{
    public class WASMTicketDTOService : ITicketDTOService
    {
        private readonly HttpClient _httpClient;

        public WASMTicketDTOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddCommentAsync(TicketCommentDTO comment, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/tickets/comments", comment);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketDTO> AddTicketAsync(TicketDTO ticket, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/tickets", ticket);
            response.EnsureSuccessStatusCode();

            TicketDTO? ticketDTO = await response.Content.ReadFromJsonAsync<TicketDTO>();
            return ticketDTO!;
        }

        public async Task ArchiveTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<string>($"api/tickets/archive/{ticketId}", "");
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCommentAsync(int commentId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"api/tickets/comments/{commentId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TicketDTO>> GetAllTicketsAsync(int companyId)
        {
            IEnumerable<TicketDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>($"api/tickets") ?? [];

            return request;
        }

        public async Task<IEnumerable<TicketDTO>> GetArchivedTickets(int companyId)
        {
            IEnumerable<TicketDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>($"api/tickets/archived") ?? [];

            return request;
        }

        public Task<TicketCommentDTO?> GetCommentByIdAsync(int commentId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<TicketDTO?> GetTicketByIdAsync(int ticketId, int companyId)
        {
            TicketDTO? ticketDTO = await _httpClient.GetFromJsonAsync<TicketDTO>($"api/tickets/{ticketId}");
            return ticketDTO;
        }

        public async Task<IEnumerable<TicketCommentDTO>> GetTicketCommentsAsync(int ticketId, int companyId)
        {
            IEnumerable<TicketCommentDTO> comments = await _httpClient.GetFromJsonAsync<IEnumerable<TicketCommentDTO>>($"api/tickets/comments/{ticketId}") ?? [];
            return comments;
        }

        public async Task RestoreTicketAsync(int ticketId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<string>($"api/tickets/restore/{ticketId}", "");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateCommentAsync(TicketCommentDTO comment, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<TicketCommentDTO>($"api/tickets/comments/{comment.Id}", comment);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateTicketAsync(TicketDTO ticket, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticket.Id}", ticket);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketAttachmentDTO> AddTicketAttachment(TicketAttachmentDTO attachment, byte[] uploadData, string contentType, int companyId)
        {
            using var formData = new MultipartFormDataContent();
            formData.Headers.ContentDisposition = new("form-data");

            var fileContent = new ByteArrayContent(uploadData);
            fileContent.Headers.ContentType = new(contentType);

            if (string.IsNullOrWhiteSpace(attachment.FileName))
            {
                formData.Add(fileContent, "file");
            }
            else
            {
                formData.Add(fileContent, "file", attachment.FileName);
            }

            formData.Add(new StringContent(attachment.Id.ToString()), nameof(attachment.Id));
            formData.Add(new StringContent(attachment.FileName ?? string.Empty), nameof(attachment.FileName));
            formData.Add(new StringContent(attachment.Description ?? string.Empty), nameof(attachment.Description));
            formData.Add(new StringContent(DateTimeOffset.Now.ToString()), nameof(attachment.Created));
            formData.Add(new StringContent(attachment.UserId ?? string.Empty), nameof(attachment.UserId));
            formData.Add(new StringContent(attachment.TicketId.ToString()), nameof(attachment.TicketId));

            var res = await _httpClient.PostAsync($"api/tickets/{attachment.TicketId}/attachments", formData);
            res.EnsureSuccessStatusCode();

            var addedAttachment = await res.Content.ReadFromJsonAsync<TicketAttachmentDTO>();
            return addedAttachment!;
        }

        public async Task DeleteTicketAttachment(int attachmentId, int companyId)
        {
            var res = await _httpClient.DeleteAsync($"api/tickets/attachments/{attachmentId}");
            res.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<TicketDTO>> GetUserTicketsAsync(int companyId, string userId)
        {
            IEnumerable<TicketDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>($"api/tickets/mytickets") ?? [];

            return request;
        }

        public async Task<IEnumerable<TicketDTO>> GetUserArchivedTicketsAsync(int companyId, string userId)
        {
            IEnumerable<TicketDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<TicketDTO>>($"api/tickets/archived/mytickets") ?? [];

            return request;
        }

        public async Task<TicketAttachmentDTO?> GetTicketAttachmentByIdAsync(int ticketAttachmentId, int companyId)
        {
            TicketAttachmentDTO? ticketAttachmentDTO = await _httpClient.GetFromJsonAsync<TicketAttachmentDTO>($"api/tickets/attachments/{ticketAttachmentId}");
            return ticketAttachmentDTO;
        }
    }
}
