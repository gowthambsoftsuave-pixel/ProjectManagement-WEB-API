using Microsoft.EntityFrameworkCore;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.DAL.Repositories
{
    public class ProjectTeamRepository : GenericRepository<ProjectTeam>, IProjectTeamRepository
    {
        public ProjectTeamRepository(ProjectManagementDbContext context) : base(context)
        {
        }

        public async Task<ProjectTeam?> GetProjectTeamByIdAsync(string projectTeamId)
        {
            return await _dbSet
                .Include(pt => pt.Project)
                .Include(pt => pt.Person)
                .FirstOrDefaultAsync(pt => pt.ProjectTeamId == projectTeamId);
        }

        public async Task<IEnumerable<ProjectTeam>> GetTeamMembersByProjectAsync(string projectId)
        {
            return await _dbSet
                .Where(pt => pt.ProjectId == projectId)
                .Include(pt => pt.Person)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTeam>> GetProjectsByPersonAsync(string personId)
        {
            return await _dbSet
                .Where(pt => pt.PersonId == personId)
                .Include(pt => pt.Project)
                .ToListAsync();
        }

        public async Task<bool> IsPersonInProjectAsync(string personId, string projectId)
        {
            return await _dbSet
                .AnyAsync(pt => pt.PersonId == personId && pt.ProjectId == projectId);
        }
    }
}
