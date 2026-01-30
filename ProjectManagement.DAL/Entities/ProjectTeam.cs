using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace ProjectManagement.DAL.Entities
{
    public class ProjectTeam
    {
        [Key]
        public int Id { get; set; }

        public string ProjectTeamId { get; set; }

        [Required]
        public string ProjectId { get; set; }

        [Required]
        public string PersonId { get; set; }

        // Navigation
        public Project Project { get; set; }
        public Person Person { get; set; }
    }
}
