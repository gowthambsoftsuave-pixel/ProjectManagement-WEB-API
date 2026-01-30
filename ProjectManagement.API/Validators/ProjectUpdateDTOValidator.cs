using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class ProjectUpdateDTOValidator : AbstractValidator<ProjectUpdateDTO>
    {
        public ProjectUpdateDTOValidator()
        {
            RuleFor(x => x.ProjectName)
                .NotEmpty().WithMessage("ProjectName is required.")
                .MaximumLength(200).WithMessage("ProjectName cannot exceed 200 characters.");
        }
    }
}
