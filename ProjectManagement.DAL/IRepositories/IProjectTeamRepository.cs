using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.IRepositories
{
    /// <summary>
    /// ProjectTeam-specific repository interface
    /// </summary>
    public interface IProjectTeamRepository : IGenericRepository<ProjectTeam>
    {
        Task<ProjectTeam?> GetProjectTeamByIdAsync(string projectTeamId);
        Task<IEnumerable<ProjectTeam>> GetTeamMembersByProjectAsync(string projectId);
        Task<IEnumerable<ProjectTeam>> GetProjectsByPersonAsync(string personId);
        Task<bool> IsPersonInProjectAsync(string personId, string projectId);
    }
}
