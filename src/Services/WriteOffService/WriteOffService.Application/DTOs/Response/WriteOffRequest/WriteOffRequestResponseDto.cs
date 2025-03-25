using System;
using System.Collections.Generic;
using WriteOffService.Application.DTOs.Base;
using WriteOffService.Application.DTOs.Response.Document;
using WriteOffService.Application.DTOs.Response.RequestStatus;
using WriteOffService.Application.DTOs.Response.WriteOffReason;

namespace WriteOffService.Application.DTOs.Response.WriteOffRequest;

public class WriteOffRequestResponseDto : BaseDto
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Guid CompanyId { get; set; }
    public RequestStatusResponseDto Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public WriteOffReasonResponseDto Reason { get; set; }
    public Guid DocumentId { get; set; }
}