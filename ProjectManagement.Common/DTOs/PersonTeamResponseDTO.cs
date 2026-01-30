using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManagement.Common.DTOs
{
    public class PersonTeamResponseDTO
    {
        public string ProjectTeamId { get; set; }
        public List<string> PersonId { get; set; }
    }
}
