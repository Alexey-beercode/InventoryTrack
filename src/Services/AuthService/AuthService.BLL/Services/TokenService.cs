using System.Security.Claims;
using AuthService.BLL.Interfaces.Services;
using AuthService.Domain.Enities;

namespace AuthService.BLL.Services;

public class TokenService:ITokenService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        throw new NotImplementedException();
    }

    public List<Claim> CreateClaims(User user, List<Role> roles)
    {
        throw new NotImplementedException();
    }
}