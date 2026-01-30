using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BLL.Interface;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            var task = await _taskService.GetTaskByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask(
            [FromBody] TaskCreateDTO dto,
            [FromQuery] string managerId
        )
        {
            if (dto == null)
                return BadRequest("Task data is required.");

            var createdTask = await _taskService.CreateTaskAsync(dto, managerId);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.TaskId }, createdTask);
        }

        // ✅ NEW BULK TASK CREATE
        [HttpPost("bulk")]
        public async Task<IActionResult> CreateTasksBulk(
            [FromBody] List<TaskCreateDTO> dtoList,
            [FromQuery] string managerId
        )
        {
            if (dtoList == null || !dtoList.Any())
                return BadRequest("Task list is required.");

            var createdTasks = await _taskService.CreateTasksBulkAsync(dtoList, managerId);
            return Ok(createdTasks);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTaskStatus(
            string id,
            [FromQuery] string userId,
            [FromBody] TaskStatusEnum status
        )
        {
            await _taskService.UpdateTaskStatusAsync(id, status, userId);
            var statusName = status.ToString();

            return Ok(new
            {
                value = status,
                name = statusName,
                message = $"Task Status updated to {statusName}"
            });
        }

        [HttpPut("{id}/reassign")]
        public async Task<IActionResult> ReassignTask(
            string id,
            [FromQuery] string managerId,
            [FromQuery] string newPersonId
        )
        {
            await _taskService.ReassignTaskAsync(id, newPersonId, managerId);
            return Ok(new { message = $"Task Reassigned to {newPersonId}" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await _taskService.DeleteTaskAsync(id);
            return Ok("Task Deleted Successfully");
        }

        [HttpGet("user/{personId}")]
        public async Task<IActionResult> GetTaskByUserAsync(string personId)
        {
            var tasks = await _taskService.GetTaskByUserAsync(personId);
            return Ok(tasks);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetTasksPaged([FromQuery]PagedRequest request)
        {
            var result = await _taskService.GetTasksPagedAsync(request);
            return Ok(result);
        }
    }
}
