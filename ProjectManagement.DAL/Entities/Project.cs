using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DAL.Entities
{
    public class Project
    {
        [Key]
        public string ProjectId { get; set; } // P001, P002...

        [Required]
        public string ProjectName { get; set; }

        public int TotalSprintCount { get; set; }

        public int CurrentSprintCount { get; set; } = 1;

        [Required]
        public string CreatedByAdminId { get; set; } // FK to Person

        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Person CreatedByAdmin { get; set; }
        public ICollection<ProjectTask> Tasks { get; set; }
        public ICollection<ProjectTeam> ProjectTeam { get; set; }
    }
}
