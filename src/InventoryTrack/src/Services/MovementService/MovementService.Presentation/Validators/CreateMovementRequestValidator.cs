using FluentValidation;
using MovementService.Application.DTOs.Request.MovementRequest;

namespace MovementService.Presentation.Validators;

public class CreateMovementRequestValidator : AbstractValidator<CreateMovementRequestDto>
{
    public CreateMovementRequestValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty()
            .WithMessage("ItemId is required");

        RuleFor(x => x.SourceWarehouseId)
            .NotEmpty()
            .WithMessage("SourceWarehouseId is required");

        RuleFor(x => x.DestinationWarehouseId)
            .NotEmpty()
            .WithMessage("DestinationWarehouseId is required")
            .NotEqual(x => x.SourceWarehouseId)
            .WithMessage("Destination warehouse must be different from source warehouse");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0");
    }
}