using ProjectManagement.Common.Enums;

namespace ProjectManagement.Common.DTOs
{
    public class TaskCreateDTO
    {
        public string TaskName { get; set; }
        public string ProjectId { get; set; }
        public string AssignedToPersonId { get; set; }
        public int? SprintNumber { get; set; }
        public RoleEnum Status { get; set; }
        
    }
}
