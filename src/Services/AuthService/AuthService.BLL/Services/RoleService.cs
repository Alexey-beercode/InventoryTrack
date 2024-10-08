using AuthService.BLL.DTOs.Implementations.Requests.UserRole;
using AuthService.BLL.DTOs.Implementations.Responses.Role;
using AuthService.BLL.Interfaces.Services;
using AuthService.DAL.Interfaces;
using AutoMapper;
using EventMaster.BLL.Exceptions;

namespace AuthService.BLL.Services;

public class RoleService:IRoleService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RoleDTO>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Roles.GetRolesByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<RoleDTO>>(roles);
    }

    public async Task SetRoleToUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _unitOfWork.Roles.SetRoleToUserAsync(userRoleDto.UserId, userRoleDto.RoleId, cancellationToken);
        if (!isSuccess)
        {
            throw new InvalidOperationException($"Role with id : {userRoleDto.RoleId} cannot be set to user with id : {userRoleDto.UserId}");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(UserRoleDTO userRoleDto, CancellationToken cancellationToken = default)
    {
        var isSuccess = await _unitOfWork.Roles.RemoveRoleFromUserAsync(userRoleDto.UserId, userRoleDto.RoleId, cancellationToken);
        if (!isSuccess)
        {
            throw new InvalidOperationException($"Role with id : {userRoleDto.RoleId} cannot be removed from user with id : {userRoleDto.UserId}");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Roles.GetAllAsync(cancellationToken);
        return roles.Select(company => _mapper.Map<RoleDTO>(company)).ToList();
    }

    public async Task<RoleDTO> GetByIdAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(roleId, cancellationToken);
        if (role==null)
        {
            throw new EntityNotFoundException("Role", roleId);
        }

        return _mapper.Map<RoleDTO>(role);
    }
}