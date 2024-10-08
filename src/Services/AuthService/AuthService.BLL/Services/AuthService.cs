using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.BLL.Services;

public class AuthService:IAuthService
{
    public Task<string> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<string> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}