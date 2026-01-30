using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class TaskUpdateDTOValidator : AbstractValidator<TaskUpdateDTO>
    {
        public TaskUpdateDTOValidator()
        {
            RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status specified.");
        
        }
    }
}
