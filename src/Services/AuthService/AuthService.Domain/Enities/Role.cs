using AuthService.Domain.Interfaces;

namespace AuthService.Domain.Enities;

public class Role:BaseEntity
{
    public string Name { get; set; }
}