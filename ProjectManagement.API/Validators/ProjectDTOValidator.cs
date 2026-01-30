using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class ProjectDTOValidator : AbstractValidator<ProjectDTO>
    {
        public ProjectDTOValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("ProjectId is required.");
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("ProjectName is required.")
                .MaximumLength(200).WithMessage("ProjectName cannot exceed 200 characters.");
            RuleFor(x => x.TotalSprintCount).GreaterThan(0).WithMessage("TotalSprintCount must be greater than 0.");
        }
    }
}
