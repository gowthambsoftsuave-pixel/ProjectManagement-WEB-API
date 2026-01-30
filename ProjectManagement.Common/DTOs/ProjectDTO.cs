public class ProjectDTO
{
    public string ProjectId { get; set; }          
    public string ProjectName { get; set; }

    public int TotalSprintCount { get; set; }
    public int CurrentSprintCount { get; set; }

    public string CreatedByAdminId { get; set; }   

    public bool IsCompleted { get; set; }
}
