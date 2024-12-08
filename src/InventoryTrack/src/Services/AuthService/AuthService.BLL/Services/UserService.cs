using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Exceptions;
using AuthService.BLL.Helpers;
using AuthService.BLL.Interfaces.Facades;
using AuthService.BLL.Interfaces.Services;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;
using EventMaster.BLL.Exceptions;

namespace AuthService.BLL.Services;

public class UserService:IUserService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserFacade _userFacade;

    public UserService(IMapper mapper, IUnitOfWork unitOfWork, IUserFacade userFacade)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userFacade = userFacade;
    }

    public async Task<IEnumerable<UserRepsonseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
        var usersDtos = new List<UserRepsonseDTO>();
        foreach (var user in users)
        {
            usersDtos.Add(await _userFacade.GetFullDtoAsync(user,cancellationToken));
        }

        return usersDtos;
    }

    public async Task<UserRepsonseDTO> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user==null)
        {
            throw new EntityNotFoundException("User", userId);
        }

        return await _userFacade.GetFullDtoAsync(user, cancellationToken);
    }

    public async Task<UserRepsonseDTO> GetByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByLoginAsync(login, cancellationToken);
        if (user==null)
        {
            throw new EntityNotFoundException($"User with login : {login} not found");
        }

        return await _userFacade.GetFullDtoAsync(user, cancellationToken);
    }

    public async Task<UserRepsonseDTO> GetByNameAsync(GetUserByNameDTO getUserByNameDto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByNameAsync(getUserByNameDto.FirstName,getUserByNameDto.LastName, cancellationToken);
        if (user==null)
        {
            throw new EntityNotFoundException($"User with name : {getUserByNameDto.FirstName} {getUserByNameDto.LastName} not found");
        }

        return await _userFacade.GetFullDtoAsync(user, cancellationToken);
    }

    public async Task<IEnumerable<UserRepsonseDTO>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var users = await _unitOfWork.Users.GetByCompanyIdAsync(companyId, cancellationToken);
        var usersDtos = new List<UserRepsonseDTO>();
        foreach (var user in users)
        {
            usersDtos.Add(await _userFacade.GetFullDtoAsync(user,cancellationToken));
        }

        return usersDtos;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user==null)
        {
            throw new EntityNotFoundException("User", userId);
        }

        await _unitOfWork.Users.DeleteAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateUserDTO userDto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userDto.Id, cancellationToken);
        if (user==null)
        {
            throw new EntityNotFoundException("User", userDto.Id);
        }

        _mapper.Map(userDto, user);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RegisterUserToCompany(RegisterUserToCompanyDTO registerUserToCompanyDto,
        CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(registerUserToCompanyDto.CompanyId, cancellationToken);
        if (company==null)
        {
            throw new EntityNotFoundException("Company", registerUserToCompanyDto.CompanyId);
        }

        var userFromDb = await _unitOfWork.Users.GetByLoginAsync(registerUserToCompanyDto.Login, cancellationToken);
        if (userFromDb!=null)
        {
            throw new AlreadyExistsException("User");
        }

        var newUser = _mapper.Map<User>(registerUserToCompanyDto);
        newUser.PasswordHash = PasswordHelper.HashPassword(registerUserToCompanyDto.Password);
        await _unitOfWork.Users.CreateAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var role = await _unitOfWork.Roles.GetByIdAsync(registerUserToCompanyDto.RoleId, cancellationToken);
        if (role==null)
        {
            throw new EntityNotFoundException("Role", registerUserToCompanyDto.RoleId);
        }

        var user = await _unitOfWork.Users.GetByLoginAsync(registerUserToCompanyDto.Login, cancellationToken);
        
        await _unitOfWork.Roles.SetRoleToUserAsync(user.Id, role.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}