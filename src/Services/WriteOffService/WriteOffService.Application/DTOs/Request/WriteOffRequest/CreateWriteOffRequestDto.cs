using Microsoft.AspNetCore.Http;
using WriteOffService.Domain.Entities;

namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class CreateWriteOffRequestDto
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Guid? ReasonId { get; set; }
    public string? AnotherReason { get; set; }
    public List<IFormFile> Documents { get; set; } = new();
    public Guid CompanyId { get; set; }
}