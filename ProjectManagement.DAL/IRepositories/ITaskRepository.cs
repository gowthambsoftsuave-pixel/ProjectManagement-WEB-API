using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.IRepositories
{
    /// <summary>
    /// Task-specific repository interface
    /// </summary>
    public interface ITaskRepository : IGenericRepository<ProjectTask>
    {
        Task<ProjectTask?> GetTaskWithDetailsAsync(string taskId);
        Task<IEnumerable<ProjectTask>> GetTasksByPersonAsync(string personId);
        Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(string projectId);
        Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(TaskStatusEnum status);
        Task<PagedResponse<ProjectTask>> GetTasksPagedAsync(PagedRequest request);
        Task<IEnumerable<ProjectTask>> GetTasksBySprintAsync(string projectId, int sprintNumber);
    }
}
