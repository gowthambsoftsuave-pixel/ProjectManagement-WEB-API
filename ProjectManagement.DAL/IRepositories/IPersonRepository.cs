using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Entities;

namespace ProjectManagement.DAL.IRepositories
{
    /// <summary>
    /// Person-specific repository interface
    /// </summary>
    public interface IPersonRepository : IGenericRepository<Person>
    {
        Task<Person?> GetPersonWithTasksAsync(string personId);
        Task<IEnumerable<Person>> GetPersonsByRoleAsync(RoleEnum role);
        Task<Person?> GetLastPersonByRoleAsync(RoleEnum role);
        Task<PagedResponse<Person>> GetPersonsPagedAsync(PagedRequest request);
        Task<IEnumerable<Person>> GetTeamMembersByProjectIdAsync(string projectId);
    }
}
