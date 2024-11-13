namespace MovementService.Application.DTOs.Request.MovementRequest;

public class ChangeStatusDto
{
    public Guid UserId { get; set; }
    public Guid RequestId { get; set; }
}