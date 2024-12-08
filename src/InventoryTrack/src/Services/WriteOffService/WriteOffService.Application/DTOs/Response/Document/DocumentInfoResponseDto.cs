using WriteOffService.Application.DTOs.Base;

namespace WriteOffService.Application.DTOs.Response.Document;

public class DocumentInfoResponseDto:BaseDto
{
    public string FileName { get; set; }
    public string FileType { get; set; }
}