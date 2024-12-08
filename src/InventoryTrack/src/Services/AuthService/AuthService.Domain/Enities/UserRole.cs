using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Enities;

public class UserRole:BaseEntity
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}