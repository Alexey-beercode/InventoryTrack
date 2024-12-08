using InventoryService.Application.DTOs.Base;

namespace InventoryService.Application.DTOs.Response.Document;

public class DocumentInfoResponseDto:BaseDto
{
    public string FileName { get; set; }
    public string FileType { get; set; }
}