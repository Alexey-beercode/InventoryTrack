using InventoryService.Application.Interfaces.Services;
using MassTransit;

namespace InventoryService.Infrastructure.Messaging.Consumers;

public class GetWarehouseConsumer : IConsumer<GetWarehouseMessage>
{
    private readonly IWarehouseService _warehouseService;

    public GetWarehouseConsumer(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    public async Task Consume(ConsumeContext<GetWarehouseMessage> context)
    {
        var id = context.Message.Id;
        var warehouse = await _warehouseService.GetByIdAsync(id);
        await context.RespondAsync(new WarehouseResponseMessage()
        {
            Name = warehouse.Name,
            Location = warehouse.Location,
            Type = warehouse.Type.Name
        });
    }
}