using WriteOffService.Domain.Common;

namespace WriteOffService.Domain.Entities;

public class WriteOffAct : BaseEntity
{
    public Guid WriteOffRequestId { get; set; }
    public Guid DocumentId { get; set; }
    
    public WriteOffRequest WriteOffRequest { get; set; }
    public Document Document { get; set; }
}