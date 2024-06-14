using BugTracker.Client.Models;
using BugTracker.Client.Services.Interfaces;
using BugTracker.Data;
using BugTracker.Helpers;
using BugTracker.Model;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class ProjectDTOService : IProjectDTOService
    {
        private readonly IProjectRepository _repository;
        private readonly ICompanyRepository _companyRepository;

        public ProjectDTOService(IProjectRepository repository, ICompanyRepository companyRepository)
        {
            _repository = repository;
            _companyRepository = companyRepository;
        }

        public async Task AddMemberToProjectAsync(int projectId, string memberId, string managerId)
        {
            await _repository.AddMemberToProjectAsync(projectId, memberId, managerId);
        }

        public async Task<ProjectDTO> AddProjectAsync(ProjectDTO project, int companyId, string userId)
        {
            Project newProject = new Project()
            {
                Name = project.Name,
                CompanyId = companyId,
                Description = project.Description,
                Created = DateTimeOffset.Now,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                Priority = project.Priority,
            };

            newProject = await _repository.AddProjectAsync(newProject, companyId, userId);

            return newProject.ToDTO();
        }

        public async Task ArchiveProjectAsync(int projectId, int companyId)
        {
            await _repository.ArchiveProjectAsync(projectId, companyId);
        }

        public async Task AssignProjectManagerAsync(int projectId, string memberId, string adminId)
        {
            await _repository.AssignProjectManagerAsync(projectId, memberId, adminId);
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync(int companyId)
        {
            IEnumerable<Project> projects = await _repository.GetAllProjectsAsync(companyId);

            IEnumerable<ProjectDTO> projectDTOs = projects.Select(p => p.ToDTO());

            return projectDTOs;
        }

        public async Task<IEnumerable<ProjectDTO>> GetArchivedProjects(int companyId)
        {
            IEnumerable<Project> projects = await _repository.GetArchivedProjects(companyId);

            IEnumerable<ProjectDTO> projectDTOs = projects.Select(p => p.ToDTO());

            return projectDTOs;
        }

        public async Task<IEnumerable<ProjectDTO>> GetMemberArchivedProjectsAsync(string userId, int companyId)
        {
            IEnumerable<Project> memberProjects = await _repository.GetMemberArchivedProjectsAsync(userId, companyId);

            IEnumerable<ProjectDTO> memberProjectDTOs = memberProjects.Select(p => p.ToDTO());

            return memberProjectDTOs;
        }

        public async Task<IEnumerable<ProjectDTO>> GetMemberProjectsAsync(string userId, int companyId)
        {
            IEnumerable<Project> memberProjects = await _repository.GetMemberProjectsAsync(userId, companyId);

            IEnumerable<ProjectDTO> memberProjectDTOs = memberProjects.Select(p => p.ToDTO());

            return memberProjectDTOs;
        }

        public async Task<ProjectDTO?> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project? project = await _repository.GetProjectByIdAsync(projectId, companyId);
            return project?.ToDTO();
        }

        public async Task<UserDTO?> GetProjectManagerAsync(int projectId, int companyId)
        {
            ApplicationUser? projectManager = await _repository.GetProjectManagerAsync(projectId, companyId);
            if (projectManager == null) return null;    

            UserDTO userDTO = projectManager.ToDTO();
            userDTO.Role = nameof(Roles.ProjectManager);

            return userDTO;
        }

        public async Task<IEnumerable<UserDTO>> GetProjectMembersAsync(int projectId, int companyId)
        {
            IEnumerable<ApplicationUser> members = await _repository.GetProjectMembersAsync(projectId, companyId);

            List<UserDTO> result = [];

            foreach(ApplicationUser user in members)
            {
                UserDTO userDTO = user.ToDTO();
                userDTO.Role = await _companyRepository.GetUserRoleAsync(user.Id, companyId);
                result.Add(userDTO);
            }

            return result;
        }

        public async Task RemoveMemberFromProjectAsync(int projectId, string memberId, string managerId)
        {
            await _repository.RemoveMemberFromProjectAsync(projectId, memberId, managerId);
        }

        public async Task RemoveProjectManagerAsync(int projectId, string adminId)
        {
            await _repository.RemoveProjectManagerAsync(projectId, adminId);
        }

        public async Task RestoreProjectAsync(int projectId, int companyId)
        {
            await _repository.RestoreProjectAsync(projectId, companyId);
        }

        public async Task UpdateProjectAsync(ProjectDTO project, int companyId)
        {
            Project? projectToUpdate = await _repository.GetProjectByIdAsync(project.Id, companyId);

            if (projectToUpdate is not null)
            {
                projectToUpdate.Name = project.Name;
                projectToUpdate.Description = project.Description;
                projectToUpdate.Priority = project.Priority;
                projectToUpdate.EndDate = project.EndDate;
                projectToUpdate.IsArchived = project.IsArchived;
                projectToUpdate.StartDate = project.StartDate;

                await _repository.UpdateProjectAsync(projectToUpdate, companyId);
            }
        }
    }
}
