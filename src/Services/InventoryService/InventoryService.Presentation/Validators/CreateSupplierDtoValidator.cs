using FluentValidation;
using InventoryService.Application.DTOs.Request.Supplier;

namespace InventoryService.Validators;

public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.PhoneNumber).NotEmpty().MaximumLength(20); // Adjust max length as needed
        RuleFor(x => x.PostalAddress).NotEmpty().MaximumLength(255);
        RuleFor(x => x.AccountNumber).NotEmpty().MaximumLength(50); // Adjust max length as needed
    }
}