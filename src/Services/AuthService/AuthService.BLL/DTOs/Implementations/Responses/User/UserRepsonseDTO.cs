using AuthService.BLL.DTOs.Implementations.Responses.Role;

namespace AuthService.BLL.DTOs.Implementations.Responses.User;

public class UserRepsonseDTO
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<RoleDTO> Roles { get; set; }
    public Guid WarehouseId { get; set; }
}