using FluentValidation;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;

namespace WriteOffService.Presentation.Validators;

public class WriteOffRequestFilterDtoValidator : AbstractValidator<WriteOffRequestFilterDto>
{
    public WriteOffRequestFilterDtoValidator()
    {
        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("PageSize must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("PageSize must not exceed 100");

        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("PageNumber must be greater than 0");
    }
}