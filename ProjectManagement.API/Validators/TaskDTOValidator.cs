using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class TaskDTOValidator : AbstractValidator<TaskDTO>
    {
        public TaskDTOValidator()
        {
            RuleFor(x => x.TaskId).NotEmpty().WithMessage("TaskId is required.");
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Task Name is required.")
                .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("ProjectId is required.");
            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status specified.");
            RuleFor(x => x.AssignedToPersonId)
                .NotEmpty().WithMessage("AssignedToPersonId is required.");
        }
    }
}
