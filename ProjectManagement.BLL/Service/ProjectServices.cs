using AutoMapper;
using ProjectManagement.BLL.Interface;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Exceptions;
using ProjectManagement.Common.Pagination;
using ProjectManagement.DAL.Entities;
using ProjectManagement.DAL.IRepositories;

namespace ProjectManagement.BLL.Service
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDTO>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDTO>>(projects);
        }

        public async Task<ProjectDTO> GetProjectByIdAsync(string projectId)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new NotFoundException($"Project with ID {projectId} not found.");

            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<ProjectDTO> CreateProjectAsync(ProjectCreateDTO dto)
        {
            var lastProject = await _projectRepository.GetLastProjectAsync();

            string nextId = lastProject != null
                ? $"P{(int.Parse(lastProject.ProjectId.Substring(1)) + 1):D3}"
                : "P001";

            if (!dto.TotalSprintCount.HasValue || dto.TotalSprintCount <= 0)
                throw new BadRequestException("TotalSprintCount must be provided and > 0");

            var project = _mapper.Map<Project>(dto);
            project.ProjectId = nextId;
            project.CurrentSprintCount = 1;
            project.IsCompleted = false;
            project.CreatedAt = DateTime.UtcNow;

            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveChangesAsync();

            return _mapper.Map<ProjectDTO>(project);
        }

        public async Task<List<ProjectDTO>> CreateProjectsBulkAsync(
            List<ProjectCreateDTO> dtoList
        )
        {
            var projectsToAdd = new List<Project>();

            var lastProject = await _projectRepository.GetLastProjectAsync();

            int startNumber = lastProject == null
                ? 1
                : int.Parse(lastProject.ProjectId.Substring(1)) + 1;

            foreach (var dto in dtoList)
            {
                if (!dto.TotalSprintCount.HasValue || dto.TotalSprintCount <= 0)
                    throw new BadRequestException("TotalSprintCount must be provided and > 0");

                var project = _mapper.Map<Project>(dto);
                project.ProjectId = $"P{startNumber:D3}";
                project.CurrentSprintCount = 1;
                project.IsCompleted = false;
                project.CreatedAt = DateTime.UtcNow;

                projectsToAdd.Add(project);
                startNumber++;
            }

            await _projectRepository.AddRangeAsync(projectsToAdd);
            await _projectRepository.SaveChangesAsync();

            return _mapper.Map<List<ProjectDTO>>(projectsToAdd);
        }

        public async Task UpdateProjectAsync(string projectId, ProjectUpdateDTO dto)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new NotFoundException($"Project with ID {projectId} not found.");

            _mapper.Map(dto, project);
            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task UpdateCurrentSprintAsync(string projectId, int currentSprint)
        {
            var project = await _projectRepository.GetByIdAsync(projectId);
            if (project == null)
                throw new NotFoundException($"Project with ID {projectId} not found.");

            if (currentSprint < project.CurrentSprintCount)
                throw new BadRequestException("Sprint Count cannot be reduced. ");

            if (currentSprint > project.TotalSprintCount)
                throw new BadRequestException("Current sprint cannot exceed total sprint count.");

            project.CurrentSprintCount = currentSprint;
            _projectRepository.Update(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task DeleteProjectAsync(string projectId)
        {
            var project = await _projectRepository.GetProjectWithDetailsAsync(projectId);

            if (project == null)
                throw new NotFoundException($"Project with ID {projectId} not found.");

            if (project.Tasks.Any(t => t.Status != TaskStatusEnum.Completed))
                throw new BadRequestException("Cannot delete project with active tasks.");

            _projectRepository.Remove(project);
            await _projectRepository.SaveChangesAsync();
        }

        public async Task<PagedResponse<ProjectDTO>> GetProjectsPagedAsync(PagedRequest request)
        {
            var pagedProjects = await _projectRepository.GetProjectsPagedAsync(request);

            return new PagedResponse<ProjectDTO>
            {
                Data = _mapper.Map<IEnumerable<ProjectDTO>>(pagedProjects.Data),
                PageNumber = pagedProjects.PageNumber,
                PageSize = pagedProjects.PageSize,
                TotalRecords = pagedProjects.TotalRecords
            };
        }
    }
}
