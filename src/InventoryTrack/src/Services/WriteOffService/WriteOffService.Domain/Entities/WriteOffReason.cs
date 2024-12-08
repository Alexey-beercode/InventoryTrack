using WriteOffService.Domain.Common;

namespace WriteOffService.Domain.Entities;

public class WriteOffReason : BaseEntity
{
    public string Reason { get; set; }
}