using Microsoft.EntityFrameworkCore;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.DAL.Repositories
{
    public class PersonRepository : GenericRepository<Person>, IPersonRepository
    {
        public PersonRepository(ProjectManagementDbContext context) : base(context)
        {
        }

        public async Task<Person?> GetPersonWithTasksAsync(string personId)
        {
            return await _dbSet
                .Include(p => p.Tasks)
                .FirstOrDefaultAsync(p => p.PersonId == personId);
        }

        public async Task<IEnumerable<Person>> GetPersonsByRoleAsync(RoleEnum role)
        {
            return await _dbSet
                .Where(p => p.Role == role)
                .ToListAsync();
        }

        public async Task<Person?> GetLastPersonByRoleAsync(RoleEnum role)
        {
            return await _dbSet
                .Where(p => p.Role == role)
                .OrderByDescending(p => p.PersonId)
                .FirstOrDefaultAsync();
        }

        public async Task<PagedResponse<Person>> GetPersonsPagedAsync(PagedRequest request)
        {
            var query = _dbSet.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(p =>
                    p.PersonId.ToLower().Contains(search) ||
                    p.Name.ToLower().Contains(search) ||
                    p.Role.ToString().ToLower().Contains(search)
                );
            }

            // Apply role filter if provided
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                if (Enum.TryParse<RoleEnum>(request.Role, true, out var roleEnum))
                {
                    query = query.Where(p => p.Role == roleEnum);
                }
            }

            // Apply sorting
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderBy(p => p.PersonId);
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

            return new PagedResponse<Person>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<Person>> GetTeamMembersByProjectIdAsync(string projectId)
        {
            return await _context.ProjectTeams
                .Where(pt => pt.ProjectId == projectId)
                .Include(pt => pt.Person)
                .Select(pt => pt.Person)
                .ToListAsync();
        }
    }
}
