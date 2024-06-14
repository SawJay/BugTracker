using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BugTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectDTOService _projectService;
        private readonly UserManager<ApplicationUser> _userManager;
        private string _userId => _userManager.GetUserId(User)!; // [authorize] means userId cannot be null

        private int? _companyId => User.FindFirst("CompanyId") != null ? int.Parse(User.FindFirst("CompanyId")!.Value) : null;

        public ProjectsController(IProjectDTOService projectService, UserManager<ApplicationUser> userManager)
        {
            _projectService = projectService;
            _userManager = userManager;
        }

        // GET: "api/projects" -> returns the users projects
        [HttpGet]
        public async Task<ActionResult<List<ProjectDTO>>> GetAllProjects()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {

                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<ProjectDTO> projects = await _projectService.GetAllProjectsAsync(_companyId.Value);

                        return Ok(projects);
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

        [HttpGet("myprojects")]
        public async Task<ActionResult<List<ProjectDTO>>> GetMemberProjects()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {

                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<ProjectDTO> projects = await _projectService.GetMemberProjectsAsync(_userId, _companyId.Value);

                        return Ok(projects);
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

        [HttpGet("archived/myprojects")]
        public async Task<ActionResult<List<ProjectDTO>>> GetMemberArchivedProjects()
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        IEnumerable<ProjectDTO> projects = await _projectService.GetMemberArchivedProjectsAsync(_userId, _companyId.Value);

                        return Ok(projects);
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
        public async Task<ActionResult<List<ProjectDTO>>> GetArchivedProjects()
        {
            try
            {
                if (_companyId is not null)
                {
                    IEnumerable<ProjectDTO> projects = await _projectService.GetArchivedProjects(_companyId.Value);

                    return Ok(projects);
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

        [HttpGet("{projectId:int}")]
        public async Task<ActionResult<ProjectDTO>> GetProjectById([FromRoute] int projectId)
        {
            try
            {
                if (_companyId is not null)
                {
                    ProjectDTO? projectDTO = await _projectService.GetProjectByIdAsync(projectId, _companyId.Value);
                    return Ok(projectDTO);
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
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<ActionResult<ProjectDTO>> AddProject([FromBody] ProjectDTO projectDTO)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    if (_companyId is not null)
                    {
                        ProjectDTO createdProjectDTO = await _projectService.AddProjectAsync(projectDTO, _companyId.Value, _userId);
                        return Ok(createdProjectDTO);
                    }
                    else { return BadRequest(); }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else { return BadRequest(); }
        }

        [HttpPut("{projectId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> UpdateProject([FromRoute] int projectId, [FromBody] ProjectDTO projectDTO)
        {
            // if person is a PM, and if any projects users ID match this ID
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

            if (User.IsInRole("ProjectManager") && user?.Id == projectManager?.Id || User.IsInRole("Admin"))
            {
                try
                {
                    if (_companyId is not null)
                    {
                        if (projectId != projectDTO.Id)
                        {
                            return BadRequest();
                        }
                        else
                        {
                            await _projectService.UpdateProjectAsync(projectDTO, _companyId.Value);
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

        [HttpPut("archive/{projectId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> ArchiveProject([FromRoute] int projectId)
        {
            // if person is a PM, and if any projects users ID match this ID
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

            if (User.IsInRole("ProjectManager") && user?.Id == projectManager?.Id || User.IsInRole("Admin"))
            {

                try
                {
                    if (_companyId is not null)
                    {
                        await _projectService.ArchiveProjectAsync(projectId, _companyId.Value);
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

        [HttpPut("restore/{projectId:int}")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> RestoreProject([FromRoute] int projectId)
        {
            // if person is a PM, and if any projects users ID match this ID
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

            if (User.IsInRole("ProjectManager") && user?.Id == projectManager?.Id || User.IsInRole("Admin"))
            {

                try
                {
                    if (_companyId is not null)
                    {
                        await _projectService.RestoreProjectAsync(projectId, _companyId.Value);
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
            else { return BadRequest(); }
        }

        [HttpGet("{projectId:int}/members")]
        public async Task<ActionResult<List<UserDTO>>> GetProjectMembers([FromRoute] int projectId)
        {
            try
            {
                IEnumerable<UserDTO> members = await _projectService.GetProjectMembersAsync(projectId, _companyId!.Value);

                return Ok(members);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("{projectId:int}/manager")]
        public async Task<ActionResult<UserDTO>> GetProjectManager([FromRoute] int projectId)
        {
            try
            {
                UserDTO? manager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

                if (manager == null)
                {
                    return NotFound();
                }

                return Ok(manager);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpPut("{projectId:int}/manager")]
        public async Task<IActionResult> RemoveProjectManager([FromRoute] int projectId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {

                bool InAdminRole = User.IsInRole("Admin");
                if (InAdminRole)
                {
                    try
                    {
                        await _projectService.RemoveProjectManagerAsync(projectId, _userId);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return Problem();
                    }
                }

                return BadRequest();
            }
            else { return BadRequest(); }
        }

        [HttpPut("{projectId:int}/members")]
        public async Task<IActionResult> RemoveProjectMembers([FromRoute] int projectId, [FromBody] string userId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

            if (user?.CompanyId == _companyId)
            {
                bool InAdminRole = User.IsInRole("Admin");
                if (InAdminRole || user.Id == projectManager?.Id)
                {
                    try
                    {
                        await _projectService.RemoveMemberFromProjectAsync(projectId, userId, _userId);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return Problem();
                    }
                }

                return BadRequest();

            }
            else { return BadRequest(); }
        }

        [HttpPut("{projectId:int}/members/add")]
        public async Task<IActionResult> AddProjectMember([FromRoute] int projectId, [FromBody] string userId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);
            UserDTO? projectManager = await _projectService.GetProjectManagerAsync(projectId, _companyId!.Value);

            if (user?.CompanyId == _companyId)
            {
                bool InAdminRole = User.IsInRole("Admin");
                if (InAdminRole || user.Id == projectManager?.Id)
                {
                    try
                    {
                        await _projectService.AddMemberToProjectAsync(projectId, userId, _userId);
                        return Ok();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return Problem();
                    }
                }

                return BadRequest();
            }

            else { return BadRequest(); }
        }

        [HttpPut("{projectId:int}/manager/add")]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> AddProjectManager([FromRoute] int projectId, [FromBody] string userId)
        {
            ApplicationUser? user = await _userManager.GetUserAsync(User);

            if (user?.CompanyId == _companyId)
            {
                try
                {
                    await _projectService.AssignProjectManagerAsync(projectId, userId, _userId);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return Problem();
                }
            }
            else { return BadRequest(); }
        }
    }
}
