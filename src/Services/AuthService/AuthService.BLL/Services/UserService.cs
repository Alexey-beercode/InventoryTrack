using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.BLL.Services;

public class UserService:IUserService
{
    public Task<IEnumerable<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserResponseDTO> GetByNameAsync(GetUserByNameDTO getUserByNameDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<UserResponseDTO>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UpdateUserDTO userDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}