using ProjectManagement.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.Common.DTOs
{
    public class PersonUpdateDTO
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public RoleEnum Role { get; set; }

        public bool IsActive { get; set; }
    }
}
