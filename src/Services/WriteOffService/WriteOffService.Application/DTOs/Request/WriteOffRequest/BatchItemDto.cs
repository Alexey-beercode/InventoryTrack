namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class BatchItemDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UniqueCode { get; set; }
    public List<WarehouseDetailDto> WarehouseDetails { get; set; } = new();
}