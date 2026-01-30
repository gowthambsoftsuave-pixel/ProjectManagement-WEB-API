using System;
using System.Collections.Generic;
using System.Text;
using ProjectManagement.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DAL.Entities
{
    public class ProjectTask
    {
        [Key]
        public string TaskId { get; set; }
        public string TaskName { get; set; }

        public string ProjectId { get; set; }
        public string AssignedToPersonId { get; set; }

        public int SprintNumber { get; set; }

        public TaskStatusEnum Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }



        // Navigation
        public Project Project { get; set; }
        public Person AssignedToPerson { get; set; }
    }
}
