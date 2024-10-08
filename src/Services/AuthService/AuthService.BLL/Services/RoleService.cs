using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using AuthService.BLL.DTOs.Implementations.Responses.Role;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.BLL.Services;

public class RoleService:IRoleService
{
    public Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}