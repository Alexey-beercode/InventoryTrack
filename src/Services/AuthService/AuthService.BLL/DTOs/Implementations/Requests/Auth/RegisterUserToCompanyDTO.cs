namespace AuthService.BLL.DTOs.Implementations.Requests.Auth;

public class RegisterUserToCompanyDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public Guid CompanyId { get; set; }
    public Guid RoleId { get; set; }
}