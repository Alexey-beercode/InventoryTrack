namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class GenerateInventoryDocumentDto
{
    public bool IsWriteOff { get; set; }          // ✅ true для списания
    public Guid InventoryItemId { get; set; }     // ID материала
    public Guid WarehouseId { get; set; }         // Откуда списано
    public Guid? SourceWarehouseId { get; set; }  // null для списания
    public int Quantity { get; set; }             // Сколько списано
}
