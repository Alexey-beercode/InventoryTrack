using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;

namespace AuthService.BLL.Interfaces.Services;

public interface IUserService
{
    public Task<IEnumerable<UserRepsonseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<UserRepsonseDTO> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<UserRepsonseDTO> GetByLoginAsync(string login, CancellationToken cancellationToken = default);

    public Task<UserRepsonseDTO> GetByNameAsync(GetUserByNameDTO getUserByNameDto,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<UserRepsonseDTO>>
        GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task UpdateAsync(UpdateUserDTO userDto, CancellationToken cancellationToken = default);
    Task RegisterUserToCompany(RegisterUserToCompanyDTO registerUserToCompanyDto,
        CancellationToken cancellationToken = default);
}