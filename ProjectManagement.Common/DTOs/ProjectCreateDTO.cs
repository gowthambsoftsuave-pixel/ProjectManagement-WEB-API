namespace ProjectManagement.Common.DTOs
{
    public class ProjectCreateDTO
    {
        public string ProjectName { get; set; } = string.Empty;

        // Nullable to detect if the user did not provide a value
        public int? TotalSprintCount { get; set; }

        public string CreatedByAdminId { get; set; } = string.Empty;

    }
}
