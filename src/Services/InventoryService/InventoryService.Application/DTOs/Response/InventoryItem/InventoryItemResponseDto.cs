using InventoryService.Application.DTOs.Base;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.DTOs.Response.InventoryItemStatus;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.DTOs.Response.InventoryItem;

public class InventoryItemResponseDto : BaseDto
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public long Quantity { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public SupplierResponseDto Supplier { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid DocumentId { get; set; }
    public InventoryItemStatusResponseDto Status { get; set; }
    public List<WarehouseDetailsDto> WarehouseDetails { get; set; }
    public DocumentInfoResponseDto DocumentInfo { get; set; }
    public string? BatchNumber { get; set; } // Номер партии
    public string? MeasureUnit { get; set; } // Единица измерения (колонка 2)
    // Количество уже есть в InventoriesItemsWarehouses
    // Цена уже есть как EstimatedValue (колонка 4)
    // Стоимость без НДС вычисляется (колонка 5)
    public decimal? VatRate { get; set; } // Ставка НДС % (колонка 6)
    // Сумма НДС вычисляется (колонка 7)
    // Стоимость с НДС вычисляется (колонка 8)
    public int? PlacesCount { get; set; } // Количество грузовых мест (колонка 9)
    public decimal? CargoWeight { get; set; } // Масса груза (колонка 10)
    public string? Notes { get; set; }
    
}
