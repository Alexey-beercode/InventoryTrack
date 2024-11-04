using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Enities;

public class User:BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    public Guid? CompanyId { get; set; }
}