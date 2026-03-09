using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TellMe.Application.DTOs;

namespace TellMe.Application.Validators
{
    public class AssignPermissionsToRoleDtoValidator : AbstractValidator<AssignPermissionsToRoleDto>
    {
        public AssignPermissionsToRoleDtoValidator()
        {
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("RoleId is required");

            RuleFor(x => x.PermissionIds)
                .NotEmpty().WithMessage("At least one permission must be assigned")
                .Must(ids => ids.Count > 0).WithMessage("At least one permission must be assigned");
        }
    }
}
