using InventoryService.Application.DTOs.Base;

namespace InventoryService.Application.DTOs.Response.Supplier;

public class SupplierResponseDto : BaseDto
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string PostalAddress { get; set; }
    public string AccountNumber { get; set; }
    public Guid CompanyId { get; set; }
}