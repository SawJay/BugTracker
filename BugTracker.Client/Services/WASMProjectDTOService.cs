using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using System.Net.Http.Json;

namespace BugTracker.Client.Services
{
    public class WASMProjectDTOService : IProjectDTOService
    {
        private readonly HttpClient _httpClient;

        public WASMProjectDTOService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task AddMemberToProjectAsync(int projectId, string memberId, string managerId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members/add", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO project, int companyId, string userId)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/projects", project);
            response.EnsureSuccessStatusCode();

            ProjectDTO? projectDTO = await response.Content.ReadFromJsonAsync<ProjectDTO>();
            return projectDTO!;
        }

        public async Task ArchiveProjectAsync(int projectId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<string>($"api/projects/archive/{projectId}", "");
            response.EnsureSuccessStatusCode();
        }

        public async Task AssignProjectManagerAsync(int projectId, string memberId, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/manager/add", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            IEnumerable<ProjectDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>($"api/projects") ?? [];

            return request;
        }

        public async Task<IEnumerable<ProjectDTO>> GetArchivedProjects(int companyId)
        {
            IEnumerable<ProjectDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>($"api/projects/archived") ?? [];

            return request;
        }

        public async Task<IEnumerable<ProjectDTO>> GetMemberArchivedProjectsAsync(string userId, int companyId)
        {
            IEnumerable<ProjectDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>($"api/projects/archived/myprojects") ?? [];

            return request;
        }

        public async Task<IEnumerable<ProjectDTO>> GetMemberProjectsAsync(string userId, int companyId)
        {
            IEnumerable<ProjectDTO> request = await _httpClient.GetFromJsonAsync<IEnumerable<ProjectDTO>>($"api/projects/myprojects") ?? [];

            return request;
        }

        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId)
        {
            ProjectDTO? projectDTO = await _httpClient.GetFromJsonAsync<ProjectDTO>($"api/projects/{projectId}");
            return projectDTO;
        }

        public async Task<UserDTO?> GetProjectManagerAsync(int projectId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/projects/{projectId}/manager");

            if (response.IsSuccessStatusCode)
            {
                UserDTO? manager = await response.Content.ReadFromJsonAsync<UserDTO>();
                return manager;
            }

            return null;
        }

        public async Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, int companyId)
        {
            IEnumerable<UserDTO> projectMembers = await _httpClient.GetFromJsonAsync<IEnumerable<UserDTO>>($"api/projects/{projectId}/members") ?? [];

            return projectMembers;
        }

        public async Task RemoveMemberFromProjectAsync(int projectId, string memberId, string managerId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/members", memberId);
            response.EnsureSuccessStatusCode();
        }

        public async Task RemoveProjectManagerAsync(int projectId, string adminId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{projectId}/manager", projectId);
            response.EnsureSuccessStatusCode();
        }

        public async Task RestoreProjectAsync(int projectId, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync<string>($"api/projects/restore/{projectId}", "");
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProjectAsync(ProjectDTO project, int companyId)
        {
            HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"api/projects/{project.Id}", project);
            response.EnsureSuccessStatusCode();
        }
    }
}
