using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class TaskCreateDTOValidator : AbstractValidator<TaskCreateDTO>
    {
        public TaskCreateDTOValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("TaskID is required.");
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("TaskName is required.")
                .MaximumLength(200).WithMessage("TaskName cannot exceed 200 characters.");
            RuleFor(x => x.AssignedToPersonId)
                .NotEmpty().WithMessage("AssignedToPersonId is required.");
        }
    }
}
