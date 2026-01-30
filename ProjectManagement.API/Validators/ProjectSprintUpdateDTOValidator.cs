using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class ProjectSprintUpdateDTOValidator : AbstractValidator<ProjectSprintUpdateDTO>
    {
        public ProjectSprintUpdateDTOValidator()
        {
            RuleFor(x => x.CurrentSprintCount)
                .GreaterThan(0).WithMessage("CurrentSprintCount must be greater than 0.");
        }
    }
}
