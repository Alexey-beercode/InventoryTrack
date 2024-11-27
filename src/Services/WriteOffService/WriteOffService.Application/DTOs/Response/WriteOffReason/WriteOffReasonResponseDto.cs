using WriteOffService.Application.DTOs.Base;

namespace WriteOffService.Application.DTOs.Response.WriteOffReason;

public class WriteOffReasonResponseDto : BaseDto
{
    public string Reason { get; set; }
}