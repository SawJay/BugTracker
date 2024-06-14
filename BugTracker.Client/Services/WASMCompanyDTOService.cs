using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace BugTracker.Client.Services
{
    public class WASMCompanyDTOService : ICompanyDTOService
    {
        private readonly HttpClient _httpClient;

        public WASMCompanyDTOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public Task AddUserToRoleAsync(string userId, string roleName, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<CompanyDTO?> GetCompanyByIdAsync(int id)
        {
            CompanyDTO? companyDTO = await _httpClient.GetFromJsonAsync<CompanyDTO>($"api/company");
            return companyDTO;
        }

        public async Task<IEnumerable<UserDTO>> GetCompanyMembersAsync(int companyId)
        {
            IEnumerable<UserDTO> companyMembers = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/company/members") ?? [];

            return companyMembers;
        }

        public async Task<string?> GetUserRoleAsync(string userId, int companyId)
        {      
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/company/currentUser/role");
                response.EnsureSuccessStatusCode();
                
                string role = await response.Content.ReadAsStringAsync();

                return role;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserDTO>> GetUsersInRoleAsync(string roleName, int companyId)
        {
            IEnumerable<UserDTO> userInRole = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/company/{roleName}/memberRoles") ?? [];
            return userInRole;
        }

        public async Task UpdateCompanyAsync(CompanyDTO company, string adminId)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.PutAsJsonAsync("api/company/edit", company);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task UpdateUserRoleAsync(UserDTO user, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/company/roles", user);
            response.EnsureSuccessStatusCode();
        }
    }
}
