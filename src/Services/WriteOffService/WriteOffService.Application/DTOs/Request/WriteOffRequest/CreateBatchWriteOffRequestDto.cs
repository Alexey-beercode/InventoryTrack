using Microsoft.AspNetCore.Http;

namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class CreateBatchWriteOffRequestDto
{
    public string BatchNumber { get; set; }
    public Guid? ReasonId { get; set; }  // Как в обычном CreateWriteOffRequestDto
    public string? AnotherReason { get; set; }  // Как в обычном CreateWriteOffRequestDto
    public Guid CompanyId { get; set; }
    public Guid RequestedByUserId { get; set; }
}