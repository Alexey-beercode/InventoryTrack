using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using AuthService.BLL.DTOs.Implementations.Responses.Role;

namespace AuthService.BLL.Interfaces.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken=default);
    Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default);
    Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken=default);
    Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken=default);
}