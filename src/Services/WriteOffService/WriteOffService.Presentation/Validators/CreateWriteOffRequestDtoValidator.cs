using FluentValidation;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;

namespace WriteOffService.Presentation.Validators;

public class CreateWriteOffRequestDtoValidator : AbstractValidator<CreateWriteOffRequestDto>
{
    public CreateWriteOffRequestDtoValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty().WithMessage("ItemId is required");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("WarehouseId is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");

        RuleFor(x => x.Documents)
            .NotNull().WithMessage("Documents list cannot be null");

        When(x => x.Documents != null, () =>
        {
            RuleForEach(x => x.Documents)
                .Must(x => x.Length <= 10485760) // 10MB in bytes
                .WithMessage("Each document must not exceed 10MB");
        });
    }
}