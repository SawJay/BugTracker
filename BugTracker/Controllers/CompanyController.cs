using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace BugTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyDTOService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;

        private int _companyId => int.Parse(User.FindFirst("CompanyId")!.Value);

        private string _userId => _userManager.GetUserId(User)!;

        public CompanyController(ICompanyDTOService companyService, UserManager<ApplicationUser> userManager)
        {
            _companyService = companyService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<CompanyDTO>> GetCompany()
        {
            CompanyDTO? company = await _companyService.GetCompanyByIdAsync(_companyId);

            if (company is null)
            {
                return NotFound();
            }

            return Ok(company);

        }

        [HttpGet("members")]
        public async Task<ActionResult<List<UserDTO>>> GetCompanyMembers()
        {
            try
            {
                IEnumerable<UserDTO> members = await _companyService.GetCompanyMembersAsync(_companyId);

                return Ok(members);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Problem();
            }
        }

        [HttpGet("currentUser/role")]
        public async Task<ActionResult<string?>> GetUserRole()
        {
            try
            {
                string? role = await _companyService.GetUserRoleAsync(_userId, _companyId);
                return Ok(role);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpGet("{roleName}/memberRoles")]
        public async Task <ActionResult<IEnumerable<UserDTO>>> GetUsersInRole([FromRoute] string roleName)
        {
            try
            {
                IEnumerable<UserDTO> members = await _companyService.GetUsersInRoleAsync(roleName, _companyId);
                return Ok(members);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest();
            }
        }

        [HttpPut("edit")]
        public async Task<IActionResult> UpdateCompany([FromBody] CompanyDTO company)
        {
            bool InAdminRole = User.IsInRole("Admin");
            if (InAdminRole)
            {
                try
                {
                    await _companyService.UpdateCompanyAsync(company, _userId);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("roles")]
        public async Task<IActionResult> UpdateUserRole([FromBody] UserDTO userDTO)
        {
            bool InAdminRole = User.IsInRole("Admin");
            if (InAdminRole)
            {
                try
                {
                    await _companyService.UpdateUserRoleAsync(userDTO, _userId);
                    return Ok();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}


