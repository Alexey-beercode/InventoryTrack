using InventoryService.Domain.Common;
using InventoryService.Domain.Enums;

namespace InventoryService.Domain.Entities;

public class InventoryItem : BaseEntity
{
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public decimal EstimatedValue { get; set; }
    public DateTime ExpirationDate { get; set; }
    public Guid SupplierId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public Guid DocumentId { get; set; }
    public InventoryItemStatus Status { get; set; }

    public Supplier Supplier { get; set; }
    public Document Document { get; set; }
    public string BatchNumber { get; set; } // Номер партии
    public string MeasureUnit { get; set; } // Единица измерения (колонка 2)
    // Количество уже есть в InventoriesItemsWarehouses
    // Цена уже есть как EstimatedValue (колонка 4)
    // Стоимость без НДС вычисляется (колонка 5)
    public decimal VatRate { get; set; } // Ставка НДС % (колонка 6)
    // Сумма НДС вычисляется (колонка 7)
    // Стоимость с НДС вычисляется (колонка 8)
    public int PlacesCount { get; set; } // Количество грузовых мест (колонка 9)
    public decimal CargoWeight { get; set; } // Масса груза (колонка 10)
    public string Notes { get; set; }
}