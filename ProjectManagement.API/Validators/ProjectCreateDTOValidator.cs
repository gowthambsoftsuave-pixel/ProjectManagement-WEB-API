using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class ProjectCreateDTOValidator : AbstractValidator<ProjectCreateDTO>
    {
        public ProjectCreateDTOValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("ProjectName is required.")
                .MaximumLength(200).WithMessage("ProjectName cannot exceed 200 characters.");
            RuleFor(x => x.TotalSprintCount).GreaterThan(0).WithMessage("TotalSprintCount must be greater than 0.");
            RuleFor(x => x.CreatedByAdminId).NotEmpty().WithMessage("CreatedByAdminId is required.");
        }
    }
}
