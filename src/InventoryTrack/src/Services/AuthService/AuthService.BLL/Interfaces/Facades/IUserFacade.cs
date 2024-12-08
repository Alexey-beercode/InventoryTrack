using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.Domain.Enities;

namespace AuthService.BLL.Interfaces.Facades;

public interface IUserFacade
{
    Task<UserRepsonseDTO> GetFullDtoAsync(User user, CancellationToken cancellationToken = default);
}