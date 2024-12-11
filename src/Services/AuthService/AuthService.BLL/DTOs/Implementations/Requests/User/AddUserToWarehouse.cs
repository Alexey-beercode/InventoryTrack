namespace AuthService.BLL.DTOs.Implementations.Requests.User;

public class AddUserToWarehouseDto
{
    public Guid UserId { get; set; }
    public Guid WarehouseId { get; set; }
}