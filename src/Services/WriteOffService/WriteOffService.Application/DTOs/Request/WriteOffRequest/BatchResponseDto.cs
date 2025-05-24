namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class BatchResponseDto
{
    public string BatchNumber { get; set; }
    public List<BatchItemDto> Items { get; set; } = new();
}