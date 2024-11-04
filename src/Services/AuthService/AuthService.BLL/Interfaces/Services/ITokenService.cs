using System.Security.Claims;
using AuthService.Domain.Enities;

namespace AuthService.BLL.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user, IEnumerable<Role> roles);
}