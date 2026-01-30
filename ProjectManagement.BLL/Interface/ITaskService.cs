using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;
using ProjectManagement.Common.Pagination;

namespace ProjectManagement.BLL.Interface
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskDTO>> GetAllTasksAsync();
        Task<TaskDTO> GetTaskByIdAsync(string taskId);
        Task<TaskDTO> CreateTaskAsync(TaskCreateDTO dto, string managerId);
        Task<List<TaskDTO>> CreateTasksBulkAsync(List<TaskCreateDTO> dtoList, string managerId);
        Task UpdateTaskStatusAsync(string taskId, TaskStatusEnum status, string userId);
        Task ReassignTaskAsync(string taskId, string newPersonId, string managerId);
        Task DeleteTaskAsync(string taskId);
        Task<IEnumerable<TaskResponseDTO>> GetTaskByUserAsync(string persnId);
        Task<PagedResponse<TaskDTO>> GetTasksPagedAsync(PagedRequest request);
    }
}
