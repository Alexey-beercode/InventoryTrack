namespace InventoryService.Application.DTOs.Request.Supplier;

public class CreateSupplierDto
{
    public string Name { get; set; }
    public string PhoneNumber { get; set; }
    public string PostalAddress { get; set; }
    public string AccountNumber { get; set; }
    public Guid CompanyId { get; set; }
}