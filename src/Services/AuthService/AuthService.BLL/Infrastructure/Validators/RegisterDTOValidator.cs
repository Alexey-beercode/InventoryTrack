using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using FluentValidation;

public class RegisterDTOValidator : AbstractValidator<RegisterDTO>
{
    public RegisterDTOValidator()
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
            .MinimumLength(6).WithMessage("Минимальная длина пароля 6 символов");
    }
}