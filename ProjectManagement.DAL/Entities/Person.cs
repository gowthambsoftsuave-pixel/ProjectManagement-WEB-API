using ProjectManagement.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DAL.Entities
{
    public class Person
    {
        [Key]
        public string PersonId { get; set; } // A001, M001, U001

        [Required]
        public string Name { get; set; }

        [Required]
        public RoleEnum Role { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<ProjectTask> Tasks { get; set; }
        public ICollection<ProjectTeam> ProjectTeams { get; set; }
    }
}
