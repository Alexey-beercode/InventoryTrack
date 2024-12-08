using AuthService.BLL.DTOs.Implementations.Requests.Company;
using FluentValidation;

public class UpdateCompanyDTOValidator : AbstractValidator<UpdateCompanyDTO>
{
    public UpdateCompanyDTOValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("ID компании обязателен");

        RuleFor(x => x.CompanyName)
            .NotEmpty().WithMessage("Название компании обязательно")
            .Length(2, 100).WithMessage("Длина названия компании должна быть от 2 до 100 символов");

        RuleFor(x => x.LegalAddress)
            .NotEmpty().WithMessage("Юридический адрес обязателен")
            .MaximumLength(200).WithMessage("Максимальная длина юридического адреса 200 символов");

        RuleFor(x => x.PostalAddress)
            .NotEmpty().WithMessage("Почтовый адрес обязателен")
            .MaximumLength(200).WithMessage("Максимальная длина почтового адреса 200 символов");

        RuleFor(x => x.ResponsiblePersonId)
            .NotEmpty().WithMessage("ID ответственного лица обязателен");
    }
}