using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.IRepositories
{
    /// <summary>
    /// Project-specific repository interface
    /// </summary>
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsAsync(string projectId);
        Task<IEnumerable<Project>> GetProjectsByAdminAsync(string adminId);
        Task<PagedResponse<Project>> GetProjectsPagedAsync(PagedRequest request);
        Task<IEnumerable<Project>> GetActiveProjectsAsync();
        Task<Project?> GetLastProjectAsync();
    }
}
