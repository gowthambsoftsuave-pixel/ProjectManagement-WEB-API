using FluentValidation;
using ProjectManagement.Common.DTOs;

namespace ProjectManagement.API.Validators
{
    public class PersonUpdateDTOValidator : AbstractValidator<PersonUpdateDTO>
    {
        public PersonUpdateDTOValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role specified.");
        }
    }
}
