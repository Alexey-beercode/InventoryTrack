using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;

namespace AuthService.BLL.Interfaces.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterDTO registerDto,CancellationToken cancellationToken=default);
    Task<string> LoginAsync(LoginDTO loginDto,CancellationToken cancellationToken=default);
}