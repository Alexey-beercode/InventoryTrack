using AuthService.BLL.DTOs.Implementations.Requests.User;
using FluentValidation;

public class UpdateUserDTOValidator : AbstractValidator<UpdateUserDTO>
{
    public UpdateUserDTOValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID пользователя обязателен");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Имя обязательно")
            .Length(2, 50).WithMessage("Длина имени должна быть от 2 до 50 символов");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Фамилия обязательна")
            .Length(2, 50).WithMessage("Длина фамилии должна быть от 2 до 50 символов");
    }
}