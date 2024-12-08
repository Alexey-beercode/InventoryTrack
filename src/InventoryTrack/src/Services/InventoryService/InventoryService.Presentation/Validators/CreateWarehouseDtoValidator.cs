using FluentValidation;
using InventoryService.Application.DTOs.Request.Warehouse;

namespace InventoryService.Validators;

public class CreateWarehouseDtoValidator : AbstractValidator<CreateWarehouseDto>
{
    public CreateWarehouseDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Location).NotEmpty().MaximumLength(255);
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.ResponsiblePersonId).NotEmpty();
    }
}