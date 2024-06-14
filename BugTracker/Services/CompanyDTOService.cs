using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.Model;
using BugTracker.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis;

namespace BugTracker.Services
{
    public class CompanyDTOService : ICompanyDTOService
    {
        private readonly ICompanyRepository _repository;

        public CompanyDTOService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int id)
        {
            Company? company = await _repository.GetCompanyByIdAsync(id);
            return company?.ToDTO();
        }

        public async Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId)
        {
            Company? company = await _repository.GetCompanyByIdAsync(companyId);

            if (company is null) return [];

            List<UserDTO> members = [];

            foreach (ApplicationUser user in company.Members)
            {
                UserDTO member = user.ToDTO();
                member.Role = await _repository.GetUserRoleAsync(user.Id, companyId);
                members.Add(member);
            }

            return members;
        }

        public async Task<string?> GetUserRoleAsync(string userId, int companyId)
        {
            Company? company = await _repository.GetCompanyByIdAsync(companyId);

            if(company is null) return null;

            UserDTO user = new();

            ApplicationUser? appUser = company.Members.FirstOrDefault(u => u.Id == userId);

            if (appUser is null) return null;

            user = appUser.ToDTO();
            user.Role = await _repository.GetUserRoleAsync(userId, companyId);

            return user.Role;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            // get the users in this role
            IEnumerable<ApplicationUser> users = await _repository.GetUsersInRoleAsync(roleName, companyId); // make them DTOs
            IEnumerable<UserDTO> userDTOs = users.Select(u => u.ToDTO());
            // we don't have to look up their role, we already know what it is
            foreach (UserDTO user in userDTOs)
            {
                // so just assign the role
                user.Role = roleName;
            }
            return userDTOs;
        }


        public async Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            Company? companyToUpdate = await _repository.GetCompanyByIdAsync(company.Id);

            if (companyToUpdate is not null)
            {
                companyToUpdate.Name = company.Name;
                companyToUpdate.Description = company.Description;
                companyToUpdate.Id = company.Id;

                if (company.ImageURL!.StartsWith("data:"))
                {
                    companyToUpdate.Image = UploadHelper.GetImageUpload(company.ImageURL);
                }
                else
                {
                    companyToUpdate.Image = null;
                }

                await _repository.UpdateCompanyAsync(companyToUpdate, adminId);
            }
        }

        public async Task UpdateUserRoleAsync(UserDTO user, string adminId)
        {
            if (string.IsNullOrEmpty(user.Role)) return;

            await _repository.AddUserToRoleAsync(user.Id!, user.Role, adminId);
        }

    }
}
