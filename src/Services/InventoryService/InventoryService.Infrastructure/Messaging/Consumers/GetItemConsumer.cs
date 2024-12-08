using InventoryService.Application.Interfaces.Services;
using MassTransit;

namespace InventoryService.Infrastructure.Messaging.Consumers;

public class GetItemConsumer : IConsumer<GetItemMessage>
{
    private readonly IInventoryItemService _inventoryItemService;

    public GetItemConsumer(IInventoryItemService inventoryItemService)
    {
        _inventoryItemService = inventoryItemService;
    }

    public async Task Consume(ConsumeContext<GetItemMessage> context)
    {
        var id = context.Message.Id;
        var item = await _inventoryItemService.GetInventoryItemAsync(id);
        await context.RespondAsync(new ItemResponseMessage()
        {
            Name = item.Name,
            DeliveryDate = item.DeliveryDate,
            ExpirationDate = item.ExpirationDate,
            Supplier = item.Supplier.Name,
            UniqueCode = item.UniqueCode,
            Quantity = item.WarehouseDetails.Sum(w=>w.Quantity)
        });
    }
}