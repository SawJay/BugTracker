using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketDTOService _ticketService;
        private readonly IProjectDTOService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        private string _userId => _userManager.GetUserId(User)!; // [authorize] means userId cannot be null

        private int? _companyId => User.FindFirst("CompanyId") != null ? int.Parse(User.FindFirst("CompanyId")!.Value) : null;

        public TicketsController(ITicketDTOService ticketService, IProjectDTOService projectService, UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _projectService = projectService;
        }

        // GET: "api/tickets" -> returns the users tickets
        [HttpGet]
        public async Task<ActionResult<List<TicketDTO>>> GetAllTickets()
        {
            try
            {
                if (_companyId is not null)
                {
                    IEnumerable<TicketDTO> tickets = await _ticketService.GetAllTicketsAsync(_companyId.Value);

                    return Ok(tickets);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("mytickets")]
        public async Task<ActionResult<List<TicketDTO>>> GetUserTickets()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<TicketDTO> tickets = await _ticketService.GetUserTicketsAsync(_companyId.Value, _userId);

                        return Ok(tickets);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else { return BadRequest(); }
        }

        [HttpGet("archived/mytickets")]
        public async Task<ActionResult<List<TicketDTO>>> GetUserArchivedTickets()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<TicketDTO> tickets = await _ticketService.GetUserArchivedTicketsAsync(_companyId.Value, _userId);

                        return Ok(tickets);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else { return BadRequest(); }
        }

        [HttpGet("archived")]
        public async Task<ActionResult<List<TicketDTO>>> GetArchivedTickets()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<TicketDTO> tickets = await _ticketService.GetArchivedTickets(_companyId.Value);

                        return Ok(tickets);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else { return BadRequest(); }
        }

        [HttpGet("{ticketId:int}")]
        public async Task<ActionResult<TicketDTO>> GetTicketById([FromRoute] int ticketId)
        {
            try
            {
                if (_companyId is not null)
                {
                    TicketDTO? ticketDTO = await _ticketService.GetTicketByIdAsync(ticketId, _companyId.Value);
                    return Ok(ticketDTO);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("attachments/{ticketAttachmentId:int}")]
        public async Task<ActionResult<TicketDTO>> GetTicketAttachmentById([FromRoute] int ticketAttachmentId)
        {
            try
            {
                if (_companyId is not null)
                {
                    TicketAttachmentDTO? ticketAttachmentDTO = await _ticketService.GetTicketAttachmentByIdAsync(ticketAttachmentId, _companyId.Value);
                    return Ok(ticketAttachmentDTO);
                }
                else
                { return BadRequest(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPost]
        public async Task<ActionResult<TicketDTO>> AddTicket([FromBody] TicketDTO ticketDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        TicketDTO createdTicketDTO = await _ticketService.AddTicketAsync(ticketDTO, _companyId.Value);
                        return Ok(createdTicketDTO);
                    }
                    else { return BadRequest(); }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else
            {
                return BadRequest(); 
            }
        }

        [HttpPut("{ticketId:int}")]
        public async Task<IActionResult> UpdateTicketAsync([FromRoute] int ticketId, [FromBody] TicketDTO ticketDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || _userId == ticketDTO.DeveloperUserId || _userId == ticketDTO.SubmitterUserId)
                {
                    try
                    {
                        if (_companyId is not null)
                        {
                            if (ticketId != ticketDTO.Id)
                            {
                                return BadRequest();
                            }
                            else
                            {
                                await _ticketService.UpdateTicketAsync(ticketDTO, _companyId.Value, _userId);
                                return Ok();
                            }
                        }
                        else
                        {
                            return BadRequest();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return Problem();
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            else { return BadRequest(); }
        }

        [HttpPut("archive/{ticketId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> ArchiveTicket([FromRoute] int ticketId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(ticketId, _companyId!.Value);
            UserDTO? manager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId!.Value);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (User.IsInRole("Admin") || user.Id == manager?.Id)
                    {
                        await _ticketService.ArchiveTicketAsync(ticketId, _companyId.Value);
                        return Ok();
                    }
                    else
                    { return BadRequest(); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("restore/{ticketId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> RestoreTicket([FromRoute] int ticketId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(ticketId, _companyId!.Value);
            UserDTO? manager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId!.Value);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (User.IsInRole("Admin") || user.Id == manager?.Id)
                    {
                        await _ticketService.RestoreTicketAsync(ticketId, _companyId.Value);
                        return Ok();
                    }
                    else
                    { return BadRequest(); }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        // get comments by ticket id - anyone
        [HttpGet("comments/{ticketId:int}")]
        public async Task<ActionResult<IEnumerable<TicketCommentDTO>>> GetComments([FromRoute] int ticketId)
        {
            try
            {
                if (_companyId is not null)
                {
                    IEnumerable<TicketCommentDTO> comments = await _ticketService.GetTicketCommentsAsync(ticketId, _companyId.Value);

                    return Ok(comments);
                }
                else { return BadRequest(); }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        // Add comment - logged in
        [HttpPost("comments")]
        public async Task<IActionResult> AddComment([FromBody] TicketCommentDTO comment)
        {
            string userId = _userManager.GetUserId(User)!;

            comment.UserId = userId;

            ApplicationUser? user = await _userManager.GetUserAsync(User);
            TicketDTO? ticket = await _ticketService.GetTicketByIdAsync(comment.TicketId, _companyId!.Value);
            UserDTO? manager = await _projectService.GetProjectManagerAsync(ticket.ProjectId, _companyId!.Value);

            if (user?.CompanyId == _companyId)
            {
                if (manager?.Id == _userId || ticket.DeveloperUserId == _userId || ticket.SubmitterUserId == _userId || User.IsInRole("Admin"))
                {
                    await _ticketService.AddCommentAsync(comment, _companyId!.Value);

                    if (comment.UserId == userId)
                    {
                        return Ok();
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                else { return NotFound(); }
            }
            else { return BadRequest(); }
        }

        // update comment - logged in, own comment, mod or admin
        [HttpPut("comments/{commentId:int}")]
        public async Task<IActionResult> UpdateComment([FromBody] TicketCommentDTO comment, [FromRoute] int commentId)
        {
            if (comment.Id != commentId)
            {
                return BadRequest();
            }

            string userId = _userManager.GetUserId(User)!;

            if (_companyId is not null)
            {
                TicketCommentDTO? commentDTO = await _ticketService.GetCommentByIdAsync(commentId, _companyId.Value);

                if (commentDTO is not null)
                {
                    if (commentDTO.UserId == userId || User.IsInRole("Admin"))
                    {
                        await _ticketService.UpdateCommentAsync(comment, _companyId.Value, userId);
                        return Ok();
                    }
                }

                return NotFound();
            }
            else { return BadRequest(); }

        }

        // delete comment - logged in, own comment, mod or admin
        [HttpDelete("comments/{commentId:int}")] // DELETE: /api/comments/5
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                string userId = _userManager.GetUserId(User)!;

                if (_companyId is not null)
                {
                    TicketCommentDTO? comment = await _ticketService.GetCommentByIdAsync(commentId, _companyId.Value);

                    if (comment?.UserId == userId || User.IsInRole("Admin"))
                    {
                        await _ticketService.DeleteCommentAsync(commentId, _companyId.Value);
                        return NoContent();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else { return NotFound(); }
            }
            else
            {
                return BadRequest();
            }

        }

        // POST: api/Tickets/5/attachments
        // NOTE: the parameters are decorated with [FromForm] because they will be sent
        // encoded as multipart/form-data and NOT the typical JSON
        [HttpPost("{id}/attachments")]
        public async Task<ActionResult<TicketAttachmentDTO>> PostTicketAttachment(int id,
                                                                                    [FromForm] TicketAttachmentDTO attachment,
                                                                                    [FromForm] IFormFile? file)
        {
            if (attachment.TicketId != id || file is null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(User);
            var ticket = await _ticketService.GetTicketByIdAsync(id, user!.CompanyId);

            if (ticket is null)
            {
                return NotFound();
            }

            attachment.UserId = user!.Id;
            attachment.Created = DateTimeOffset.Now;

            if (string.IsNullOrWhiteSpace(attachment.FileName))
            {
                attachment.FileName = file.FileName;
            }

            // ImageHelper was renamed to UploadHelper!
            FileUpload upload = await UploadHelper.GetImageUploadAsync(file);

            try
            {
                var newAttachment = await _ticketService.AddTicketAttachment(attachment, upload.Data!, upload.Type!, user!.CompanyId);
                return Ok(newAttachment);
            }
            catch
            {
                return Problem();
            }
        }

        // DELETE: api/Tickets/attachments/1
        [HttpDelete("attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteTicketAttachment(int attachmentId)
        {
            var user = await _userManager.GetUserAsync(User);

            TicketAttachmentDTO? ticketAttachment = await _ticketService.GetTicketAttachmentByIdAsync(attachmentId, user!.CompanyId);

            if (user?.CompanyId == _companyId && User.IsInRole("Admin") || user?.Id == ticketAttachment?.UserId)
            {
                await _ticketService.DeleteTicketAttachment(attachmentId, user!.CompanyId);

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
