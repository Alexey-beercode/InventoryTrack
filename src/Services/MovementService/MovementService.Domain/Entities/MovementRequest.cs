using MovementService.Domain.Common;
using MovementService.Domain.Enums;

namespace MovementService.Domain.Entities;

public class MovementRequest : BaseEntity
{
    public Guid ItemId { get; set; }  
    public Guid SourceWarehouseId { get; set; }
    public Guid DestinationWarehouseId { get; set; }
    public long Quantity { get; set; }
    public MovementRequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid ApprovedByUserId { get; set; }
}