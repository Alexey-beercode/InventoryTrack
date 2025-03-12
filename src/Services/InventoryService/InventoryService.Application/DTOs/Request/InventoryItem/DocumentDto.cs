namespace InventoryService.Application.DTOs.Request.InventoryItem;

public class DocumentDto
{
    public string FileBase64 { get; set; } // Base64-строка файла
    public string FileName { get; set; } // Название файла
    public string FileType { get; set; } // MIME-тип файла

}