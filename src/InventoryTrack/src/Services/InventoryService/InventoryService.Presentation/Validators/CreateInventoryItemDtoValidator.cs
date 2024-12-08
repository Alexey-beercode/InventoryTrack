using FluentValidation;
using InventoryService.Application.DTOs.Request.InventoryItem;

namespace InventoryService.Validators;

public class CreateInventoryItemDtoValidator : AbstractValidator<CreateInventoryItemDto>
{
    public CreateInventoryItemDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.UniqueCode).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EstimatedValue).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ExpirationDate).GreaterThan(DateTime.UtcNow);
        RuleFor(x => x.SupplierId).NotEmpty();
        RuleFor(x => x.WarehouseId).NotEmpty();
        RuleFor(x => x.DeliveryDate).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.DocumentFile).NotNull().Must(BeAValidFile).WithMessage("Invalid file provided.");
    }
    private bool BeAValidFile(IFormFile file)
    {
        return file != null && file.Length > 0; 
    }
}