using MovementService.Application.DTOs.Base;
using MovementService.Application.DTOs.Response.MovementRequestStatus;

namespace MovementService.Application.DTOs.Response.MovementRequest;

public class MovementRequestResponseDto : BaseDto
{
    public Guid ItemId { get; set; }  
    public Guid SourceWarehouseId { get; set; }
    public Guid DestinationWarehouseId { get; set; }
    public long Quantity { get; set; }
    public MovementRequestStatusResponseDto Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid? ApprovedByUserId { get; set; }
}