using FluentValidation;
using ProjectManagement.Common.DTOs;
using ProjectManagement.Common.Enums;

namespace ProjectManagement.API.Validators

{
    public class PersonCreateDTOValidators : AbstractValidator<PersonCreateDTO>
    {
        public PersonCreateDTOValidators()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
            RuleFor(x => x.Role)
                .IsInEnum().WithMessage("Invalid role specified.");
        }
    }
}
