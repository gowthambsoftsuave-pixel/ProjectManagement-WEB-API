using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Interface;
using ProjectManagement.BLL.Service;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(string id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectCreateDTO dto)
        {
            if (dto == null)
                return BadRequest("Project data is required.");

            var created = await _projectService.CreateProjectAsync(dto);
            return CreatedAtAction(nameof(GetProjectById), new { id = created.ProjectId }, created);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateProjectsBulk(
            [FromBody] List<ProjectCreateDTO> dtoList
        )
        {
            if (dtoList == null || !dtoList.Any())
                return BadRequest("Project list is required.");

            var createdProjects = await _projectService.CreateProjectsBulkAsync(dtoList);
            return Ok(createdProjects);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] ProjectUpdateDTO dto)
        {
            if (dto == null)
                return BadRequest("Project data is required.");

            await _projectService.UpdateProjectAsync(id, dto);
            return Ok("Updated Successfully");
        }

        [HttpPut("{id}/sprint")]
        public async Task<IActionResult> UpdateSprint(string id, [FromQuery] int currentSprint)
        {
            await _projectService.UpdateCurrentSprintAsync(id, currentSprint);
            return Ok("Updated the Sprint Count Successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            await _projectService.DeleteProjectAsync(id);
            return Ok("Deleted the project successfully");
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetProjectsPagedAsync([FromQuery] PagedRequest request)
        {
            var result = await _projectService.GetProjectsPagedAsync(request);
            return Ok(result);
        }
    }
}
