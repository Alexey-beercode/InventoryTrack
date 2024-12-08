using InventoryService.Application.DTOs.Request.InventoryItem;
using MassTransit;
using InventoryService.Application.Interfaces.Services;

namespace InventoryService.Infrastructure.Messaging.Consumers
{
    public class MovementsRequestConsumer : IConsumer<MoveInventoryMessage>
    {
        private readonly IInventoryItemService _inventoryItemService;

        public MovementsRequestConsumer(IInventoryItemService inventoryItemService)
        {
            _inventoryItemService = inventoryItemService;
        }

        public async Task Consume(ConsumeContext<MoveInventoryMessage> context)
        {
            var message = context.Message;
            var moveDto = new MoveItemDto()
            {
                DestinationWarehouseId = message.DestinationWarehouseId,
                ItemId = message.ItemId,
                Quantity = message.Quantity,
                SourceWarehouseId = message.SourceWarehouseId
            };
            await _inventoryItemService.MoveItemAsync(moveDto);
        }
    }
}