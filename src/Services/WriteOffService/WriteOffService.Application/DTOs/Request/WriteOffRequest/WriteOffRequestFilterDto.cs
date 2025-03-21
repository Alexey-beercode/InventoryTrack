﻿using WriteOffService.Domain.Enums;

namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class WriteOffRequestFilterDto
{
    public Guid ItemId { get; set; }
    public Guid WarehouseId { get; set; }
    public long Quantity { get; set; }
    public Guid ReasonId { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public int PageSize { get; set; } = 10;
    public int PageNumber { get; set; } = 1;
    public Guid CompanyId { get; set; }
    public bool IsPaginated { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}