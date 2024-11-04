using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using FluentValidation;

public class RegisterUserToCompanyDTOValidator : AbstractValidator<RegisterUserToCompanyDTO>
{
    public RegisterUserToCompanyDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .Length(2, 50).WithMessage("Длина имени должна быть от 2 до 50 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .Length(2, 50).WithMessage("Длина фамилии должна быть от 2 до 50 символов");

        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин обязателен")
            .Length(3, 50).WithMessage("Длина логина должна быть от 3 до 50 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Минимальная длина пароля 6 символов")
            .Matches("[A-Z]").WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Matches("[a-z]").WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Matches("[0-9]").WithMessage("Пароль должен содержать хотя бы одну цифру");

        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("ID компании обязателен");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("ID роли обязателен");
    }
}