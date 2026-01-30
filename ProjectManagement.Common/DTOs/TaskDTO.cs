using ProjectManagement.Common.Enums;

namespace ProjectManagement.Common.DTOs
{
    public class TaskDTO
    {
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public string ProjectId { get; set; }
        public string AssignedToPersonId { get; set; }
        public int SprintNumber { get; set; }
        public TaskStatusEnum Status { get; set; }
    }
}
