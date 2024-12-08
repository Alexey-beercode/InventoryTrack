namespace AuthService.BLL.DTOs.Implementations.Requests.Auth;

public class RegisterDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
}