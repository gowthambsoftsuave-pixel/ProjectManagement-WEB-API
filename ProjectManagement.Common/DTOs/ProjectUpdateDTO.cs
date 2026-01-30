using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProjectManagement.Common.DTOs
{
    public class ProjectUpdateDTO
    {
        public string ProjectName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; } = false;
    }
}
