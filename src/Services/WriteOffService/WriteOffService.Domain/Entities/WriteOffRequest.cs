using WriteOffService.Domain.Common;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Domain.Entities;

public class WriteOffRequest : BaseEntity
{ 
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Guid? ReasonId { get; set; }
    public Guid CompanyId { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public string? BatchNumber { get; set; } // Номер партии
    public WriteOffReason Reason { get; set; }
    public Guid DocumentId { get; set; }
}