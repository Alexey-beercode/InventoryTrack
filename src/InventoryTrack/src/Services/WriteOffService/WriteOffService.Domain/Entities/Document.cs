using WriteOffService.Domain.Common;

namespace WriteOffService.Domain.Entities;

public class Document : BaseEntity
{
    public string FileName { get; set; }
    public string FileType { get; set; }
    public byte[] FileData { get; set; }
}