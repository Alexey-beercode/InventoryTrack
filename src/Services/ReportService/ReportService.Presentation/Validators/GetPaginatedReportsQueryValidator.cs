using FluentValidation;
using ReportService.Application.UseCases.Report.GetPaginated;

namespace ReportService.Presentation.Validators;

public class GetPaginatedReportsQueryValidator : AbstractValidator<GetPaginatedReportsQuery>
{
    public GetPaginatedReportsQueryValidator()
    {
        RuleFor(query => query.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Номер страницы должен быть больше или равен 1.");

        RuleFor(query => query.PageSize)
            .GreaterThan(0).WithMessage("Размер страницы должен быть больше 0.")
            .LessThanOrEqualTo(100).WithMessage("Размер страницы не может превышать 100.");
    }
}