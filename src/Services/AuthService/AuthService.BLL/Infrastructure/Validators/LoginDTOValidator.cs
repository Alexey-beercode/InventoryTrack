using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using FluentValidation;

public class LoginDTOValidator : AbstractValidator<LoginDTO>
{
    public LoginDTOValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Логин обязателен")
            .Length(3, 50).WithMessage("Длина логина должна быть от 3 до 50 символов");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обязателен")
            .MinimumLength(6).WithMessage("Минимальная длина пароля 6 символов");
    }
}