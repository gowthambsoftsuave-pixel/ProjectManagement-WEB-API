using Microsoft.EntityFrameworkCore;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.DAL.Repositories
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {
        public ProjectRepository(ProjectManagementDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsAsync(string projectId)
        {
            return await _dbSet
                .Include(p => p.Tasks)
                .Include(p => p.ProjectTeam)
                    .ThenInclude(pt => pt.Person)
                .Include(p => p.CreatedByAdmin)
                .FirstOrDefaultAsync(p => p.ProjectId == projectId);
        }

        public async Task<IEnumerable<Project>> GetProjectsByAdminAsync(string adminId)
        {
            return await _dbSet
                .Where(p => p.CreatedByAdminId == adminId)
                .ToListAsync();
        }

        public async Task<PagedResponse<Project>> GetProjectsPagedAsync(PagedRequest request)
        {
            var query = _dbSet.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p =>
                    p.ProjectId.ToLower().Contains(search) ||
                    p.ProjectName.ToLower().Contains(search)
                );
            }

            // Apply sorting
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderBy(p => p.ProjectId);
            }
            else
            {
                query = query.ApplySorting(request.SortBy, request.SortDirection);
            }

            // Get total count
            var totalRecords = await query.CountAsync();

            // Apply pagination
            var data = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResponse<Project>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<Project>> GetActiveProjectsAsync()
        {
            return await _dbSet
                .Where(p => !p.IsCompleted)
                .ToListAsync();
        }

        public async Task<Project?> GetLastProjectAsync()
        {
            return await _dbSet
                .OrderByDescending(p => p.ProjectId)
                .FirstOrDefaultAsync();
        }
    }
}
