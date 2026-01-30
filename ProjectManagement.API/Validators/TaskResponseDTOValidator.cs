using FluentValidation;
using ProjectManagement.Common.DTOs;    

namespace ProjectManagement.API.Validators
{
    public class TaskResponseDTOValidator : AbstractValidator<TaskResponseDTO>
    {
        public TaskResponseDTOValidator()
        {

        }
    }
}
