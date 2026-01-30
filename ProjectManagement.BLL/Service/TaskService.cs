using Microsoft.EntityFrameworkCore;
using ProjectManagement.BLL.Interface;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Exceptions;
using ProjectManagement.DAL.Entities;
using AutoMapper;
using ProjectTaskEntity = ProjectManagement.DAL.Entities.ProjectTask;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.BLL.Service
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectTeamRepository _projectTeamRepository;
        private readonly IMapper _mapper;

        public TaskService(
            ITaskRepository taskRepository,
            IPersonRepository personRepository,
            IProjectRepository projectRepository,
            IProjectTeamRepository projectTeamRepository,
            IMapper mapper)
        {
            _taskRepository = taskRepository;
            _personRepository = personRepository;
            _projectRepository = projectRepository;
            _projectTeamRepository = projectTeamRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TaskDTO>> GetAllTasksAsync()
        {
            var tasks = await _taskRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TaskDTO>>(tasks);
        }

        public async Task<TaskDTO> GetTaskByIdAsync(string taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException("Task not found");

            return _mapper.Map<TaskDTO>(task);
        }

        public async Task<TaskDTO> CreateTaskAsync(TaskCreateDTO dto, string managerId)
        {
            var result = await CreateTasksBulkAsync(new List<TaskCreateDTO> { dto }, managerId);
            return result.First();
        }

        public async Task<List<TaskDTO>> CreateTasksBulkAsync(
            List<TaskCreateDTO> dtoList,
            string managerId
        )
        {
            // Validate manager
            var manager = await _personRepository.GetByIdAsync(managerId);
            if (manager == null || manager.Role != RoleEnum.Manager)
                throw new BadRequestException("Only managers can create tasks");

            // Get all unique project IDs and validate projects exist
            var projectIds = dtoList.Select(d => d.ProjectId).Distinct().ToList();
            var projects = new Dictionary<string, Project>();
            foreach (var projectId in projectIds)
            {
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                    throw new NotFoundException($"Project {projectId} not found");
                projects[projectId] = project;
            }

            // Get all unique user IDs and validate users exist
            var userIds = dtoList.Select(d => d.AssignedToPersonId).Distinct().ToList();
            var users = new Dictionary<string, Person>();
            foreach (var userId in userIds)
            {
                var user = await _personRepository.GetByIdAsync(userId);
                if (user == null || user.Role != RoleEnum.User)
                    throw new BadRequestException("Tasks can be assigned only to Users");
                users[userId] = user;
            }

            // Validate sprint numbers
            foreach (var dto in dtoList)
            {
                int sprint = dto.SprintNumber ?? 1;
                if (sprint > projects[dto.ProjectId].TotalSprintCount)
                    throw new BadRequestException("Invalid sprint number");
            }

            // Generate next task ID
            var lastTask = await _taskRepository.FirstOrDefaultAsync(t => true);
            var allTasks = await _taskRepository.GetAllAsync();
            var orderedTasks = allTasks.OrderByDescending(t => t.TaskId).ToList();
            
            int taskCounter = orderedTasks.Any()
                ? int.Parse(orderedTasks.First().TaskId.Substring(1)) + 1
                : 1;

            // Get existing project teams
            var existingTeams = new List<ProjectTeam>();
            foreach (var projectId in projectIds)
            {
                var teams = await _projectTeamRepository.GetTeamMembersByProjectAsync(projectId);
                existingTeams.AddRange(teams);
            }

            var projectTeamMap = existingTeams
                .GroupBy(pt => pt.ProjectId)
                .ToDictionary(g => g.Key, g => g.First().ProjectTeamId);

            // Get last team ID
            var allTeams = await _projectTeamRepository.GetAllAsync();
            var lastTeam = allTeams.OrderByDescending(pt => pt.ProjectTeamId).FirstOrDefault();

            int teamCounter = lastTeam == null
                ? 1
                : int.Parse(lastTeam.ProjectTeamId.Substring(2)) + 1;

            var tasksToAdd = new List<ProjectTaskEntity>();

            // Create tasks and teams
            foreach (var dto in dtoList)
            {
                var task = _mapper.Map<ProjectTaskEntity>(dto);
                task.TaskId = $"T{taskCounter:D3}";
                task.Status = TaskStatusEnum.NotStarted;
                task.CreatedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;

                tasksToAdd.Add(task);
                taskCounter++;

                // Handle project team
                if (!projectTeamMap.TryGetValue(dto.ProjectId, out string projectTeamId))
                {
                    projectTeamId = $"PT{teamCounter:D3}";
                    teamCounter++;
                    projectTeamMap[dto.ProjectId] = projectTeamId;
                }

                // Check if team member already exists
                bool alreadyExists = existingTeams.Any(pt =>
                    pt.ProjectId == dto.ProjectId &&
                    pt.PersonId == dto.AssignedToPersonId
                );

                if (!alreadyExists)
                {
                    var newTeam = new ProjectTeam
                    {
                        ProjectTeamId = projectTeamId,
                        ProjectId = dto.ProjectId,
                        PersonId = dto.AssignedToPersonId
                    };

                    existingTeams.Add(newTeam);
                    await _projectTeamRepository.AddAsync(newTeam);
                }
            }

            await _taskRepository.AddRangeAsync(tasksToAdd);
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<List<TaskDTO>>(tasksToAdd);
        }

        public async Task UpdateTaskStatusAsync(string taskId, TaskStatusEnum status, string userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException("Task not found");

            var user = await _personRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            if (user.Role == RoleEnum.User && task.AssignedToPersonId != userId)
                throw new BadRequestException("User can update only their own tasks");

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task ReassignTaskAsync(string taskId, string newPersonId, string managerId)
        {
            var manager = await _personRepository.GetByIdAsync(managerId);
            if (manager == null || manager.Role != RoleEnum.Manager)
                throw new BadRequestException("Only managers can reassign tasks");

            var user = await _personRepository.GetByIdAsync(newPersonId);
            if (user == null || user.Role != RoleEnum.User)
                throw new BadRequestException("Tasks can be reassigned only to Users");

            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException("Task not found");

            task.AssignedToPersonId = newPersonId;
            task.UpdatedAt = DateTime.UtcNow;

            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task DeleteTaskAsync(string taskId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null)
                throw new NotFoundException("Task not found");

            _taskRepository.Remove(task);
            await _taskRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskResponseDTO>> GetTaskByUserAsync(string personId)
        {
            if (personId == null)
                throw new NotFoundException("User Id is not found");

            var tasks = await _taskRepository.GetTasksByPersonAsync(personId);

            if (!tasks.Any() || tasks == null)
                throw new NotFoundException("No tasks found");
            
            return tasks.Select(t => new TaskResponseDTO
            {
                TaskId = t.TaskId,
                TaskName = t.TaskName,
                ProjectId = t.ProjectId
            }).ToList();
        }

        public async Task<PagedResponse<TaskDTO>> GetTasksPagedAsync(PagedRequest request)
        {
            var pagedTasks = await _taskRepository.GetTasksPagedAsync(request);

            return new PagedResponse<TaskDTO>
            {
                Data = _mapper.Map<IEnumerable<TaskDTO>>(pagedTasks.Data),
                PageNumber = pagedTasks.PageNumber,
                PageSize = pagedTasks.PageSize,
                TotalRecords = pagedTasks.TotalRecords
            };
        }
    }
}
