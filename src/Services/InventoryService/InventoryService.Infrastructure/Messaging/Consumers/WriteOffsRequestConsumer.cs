using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.InventoryItem;
using MassTransit;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;

namespace InventoryService.Infrastructure.Messaging.Consumers
{
    public class WriteOffsRequestConsumer : IConsumer<WriteOffInventoryMessage>
    {
        private readonly IInventoryItemService _inventoryItemService;

        public WriteOffsRequestConsumer(IInventoryItemService inventoryItemService)
        {
            _inventoryItemService = inventoryItemService;
        }

        public async Task Consume(ConsumeContext<WriteOffInventoryMessage> context)
        {
            var message = context.Message;
            await _inventoryItemService.WriteOffItemAsync(message.ItemId, message.WarehouseId, message.Quantity);
        }
    }
}