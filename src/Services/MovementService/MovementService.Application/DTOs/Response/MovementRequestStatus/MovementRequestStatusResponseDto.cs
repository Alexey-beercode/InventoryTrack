namespace MovementService.Application.DTOs.Response.MovementRequestStatus;

public class MovementRequestStatusResponseDto
{
    public Domain.Enums.MovementRequestStatus Value { get; set; }
    public string Name { get; set; }
}