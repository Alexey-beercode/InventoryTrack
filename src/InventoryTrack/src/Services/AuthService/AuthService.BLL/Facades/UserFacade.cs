using AuthService.BLL.DTOs.Implementations.Responses.Role;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Interfaces.Facades;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;

namespace AuthService.BLL.Facades;

public class UserFacade : IUserFacade
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserFacade(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<UserRepsonseDTO> GetFullDtoAsync(User user, CancellationToken cancellationToken = default)
    {
        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(user.Id, cancellationToken);
        var rolesDto = _mapper.Map<IEnumerable<RoleDTO>>(rolesByUser);
        var userDto = _mapper.Map<UserRepsonseDTO>(user);
        userDto.Roles = (List<RoleDTO>)rolesDto;
        return userDto;
    }
}