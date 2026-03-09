using FluentValidation;
using TellMe.Application.DTOs;
using TellMe.Application.Features.Role;

namespace TellMe.Application.Validators
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Role name is required")
                .MaximumLength(100).WithMessage("Role name must not exceed 100 characters");
        }
    }
}
