using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.Auth;

namespace AuthService.BLL.Interfaces.Services;

public interface IAuthService
{
    Task<AuthReponseDTO> RegisterAsync(RegisterDTO registerDto,CancellationToken cancellationToken=default);
    Task<AuthReponseDTO> LoginAsync(LoginDTO loginDto,CancellationToken cancellationToken=default);
}