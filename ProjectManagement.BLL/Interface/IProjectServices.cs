using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.BLL.Interface
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync();
        Task<ProjectDTO> GetProjectByIdAsync(string projectId);
        Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO dto);
        Task<List<ProjectDTO>> CreateProjectsBulkAsync(List<ProjectCreateDTO> dtoList);
        Task UpdateProjectAsync(string projectId, ProjectUpdateDTO dto);
        Task UpdateCurrentSprintAsync(string projectId, int currentSprint);
        Task DeleteProjectAsync(string projectId);
        Task <PagedResponse<ProjectDTO>> GetProjectsPagedAsync(PagedRequest request);
    }
}
