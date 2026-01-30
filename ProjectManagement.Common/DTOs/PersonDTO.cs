using ProjectManagement.Common.Enums;   

namespace ProjectManagement.Common.DTOs
{
    public class PersonDTO
    {
        public string PersonId { get; set; }
        public string Name { get; set; }
        public RoleEnum Role { get; set; }
        public bool IsActive { get; set; }

    }
}
