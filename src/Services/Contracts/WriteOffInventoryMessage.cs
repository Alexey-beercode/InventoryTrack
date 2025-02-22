namespace Contracts;

public class WriteOffInventoryMessage
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
}