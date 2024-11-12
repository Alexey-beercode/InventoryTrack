using FluentValidation;
using InventoryService.Application.DTOs.Request.InventoryItem;

namespace InventoryService.Validators;

public class FilterInventoryItemDtoValidator : AbstractValidator<FilterInventoryItemDto>
{
    public FilterInventoryItemDtoValidator()
    {
        RuleFor(x => x.Name).MaximumLength(255);
        When(x => x.ExpirationDateFrom.HasValue, () =>
        {
            RuleFor(x => x.ExpirationDateTo).GreaterThanOrEqualTo(x => x.ExpirationDateFrom);
        });
        RuleFor(x => x.EstimatedValue).GreaterThanOrEqualTo(0);

    }
}