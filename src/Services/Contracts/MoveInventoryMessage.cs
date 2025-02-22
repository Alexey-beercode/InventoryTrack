namespace Contracts
{
    public class MoveInventoryMessage
    {
        public Guid ItemId { get; set; }
        public Guid SourceWarehouseId { get; set; }
        public Guid DestinationWarehouseId { get; set; }
        public long Quantity { get; set; }
    }
}