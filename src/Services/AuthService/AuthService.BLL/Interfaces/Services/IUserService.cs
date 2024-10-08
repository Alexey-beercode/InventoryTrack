using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;

namespace AuthService.BLL.Interfaces.Services;

public interface IUserService
{
    public Task<IEnumerable<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    public Task<UserResponseDTO> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task<UserResponseDTO> GetByLoginAsync(string login, CancellationToken cancellationToken = default);

    public Task<UserResponseDTO> GetByNameAsync(GetUserByNameDTO getUserByNameDto,
        CancellationToken cancellationToken = default);

    public Task<IEnumerable<UserResponseDTO>>
        GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

    public Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default);
    public Task UpdateAsync(UpdateUserDTO userDto, CancellationToken cancellationToken = default);
}