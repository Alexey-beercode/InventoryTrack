namespace AuthService.BLL.DTOs.Implementations.Requests.User;

public class UpdateUserDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}