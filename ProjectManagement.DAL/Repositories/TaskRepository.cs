using Microsoft.EntityFrameworkCore;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Data;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.DAL.Repositories
{
    public class TaskRepository : GenericRepository<ProjectTask>, ITaskRepository
    {
        public TaskRepository(ProjectManagementDbContext context) : base(context)
        {
        }

        public async Task<ProjectTask?> GetTaskWithDetailsAsync(string taskId)
        {
            return await _dbSet
                .Include(t => t.AssignedToPerson)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByPersonAsync(string personId)
        {
            return await _dbSet
                .Where(t => t.AssignedToPersonId == personId)
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByProjectAsync(string projectId)
        {
            return await _dbSet
                .Where(t => t.ProjectId == projectId)
                .Include(t => t.AssignedToPerson)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksByStatusAsync(TaskStatusEnum status)
        {
            return await _dbSet
                .Where(t => t.Status == status)
                .Include(t => t.AssignedToPerson)
                .Include(t => t.Project)
                .ToListAsync();
        }

        public async Task<PagedResponse<ProjectTask>> GetTasksPagedAsync(PagedRequest request)
        {
            var query = _dbSet.AsNoTracking();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(t =>
                    t.TaskId.ToLower().Contains(search) ||
                    t.TaskName.ToLower().Contains(search) ||
                    t.ProjectId.ToLower().Contains(search) ||
                    t.AssignedToPersonId.ToLower().Contains(search)
                );
            }

            // Apply status filter if provided
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (Enum.TryParse<TaskStatusEnum>(request.Status, true, out var statusEnum))
                {
                    query = query.Where(t => t.Status == statusEnum);
                }
            }

            // Apply sorting
            if (string.IsNullOrWhiteSpace(request.SortBy))
            {
                query = query.OrderBy(t => t.TaskId);
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

            return new PagedResponse<ProjectTask>
            {
                Data = data,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<IEnumerable<ProjectTask>> GetTasksBySprintAsync(string projectId, int sprintNumber)
        {
            return await _dbSet
                .Where(t => t.ProjectId == projectId && t.SprintNumber == sprintNumber)
                .Include(t => t.AssignedToPerson)
                .ToListAsync();
        }
    }
}
