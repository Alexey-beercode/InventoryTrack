using Microsoft.AspNetCore.Http;
using WriteOffService.Application.DTOs.Base;

namespace WriteOffService.Application.DTOs.Request.WriteOffRequest;

public class ApproveWriteOffRequestDto : BaseDto
{
    public Guid ApprovedByUserId { get; set; }
    public List<IFormFile> Documents { get; set; } = new();
}
