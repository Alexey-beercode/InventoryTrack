using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using FluentValidation;

namespace EventMaster.BLL.Infrastructure.Validators;

public class UserRoleDTOValidator : AbstractValidator<UserRoleDTO>
{
    public UserRoleDTOValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("RoleId is required");
    }
}