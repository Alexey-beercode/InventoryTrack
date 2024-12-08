using InventoryService.Domain.Common;

namespace InventoryService.Domain.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public byte[] FileData { get; set; }
}