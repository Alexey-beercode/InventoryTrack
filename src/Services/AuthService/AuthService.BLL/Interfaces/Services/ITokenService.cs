using System.Security.Claims;
using AuthService.Domain.Enities;

namespace AuthService.BLL.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    List<Claim> CreateClaims(User user,List<Role> roles);
}