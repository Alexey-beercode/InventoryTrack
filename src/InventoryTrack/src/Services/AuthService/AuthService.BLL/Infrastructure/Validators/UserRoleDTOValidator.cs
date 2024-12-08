using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using FluentValidation;

namespace AuthService.BLL.Infrastructure.Validators;

public class UserRoleDTOValidator : AbstractValidator<UserRoleDTO>
{
    public UserRoleDTOValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("ID пользователя обязателен");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("ID роли обязателен");
    }
}