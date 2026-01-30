using ProjectManagement.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Common.DTOs
{
    public class TaskUpdateDTO
    {
        [Required]
        public TaskStatusEnum Status { get; set; }
    }
}
