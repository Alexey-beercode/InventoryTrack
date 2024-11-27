using WriteOffService.Application.DTOs.Base;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class UpdateWriteOffRequestDto : BaseDto
{
    public RequestStatus Status { get; set; }
    public Guid? ApprovedByUserId { get; set; }
}