using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Responses.Auth;
using AuthService.BLL.Exceptions;
using AuthService.BLL.Helpers;
using AuthService.BLL.Interfaces.Services;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;
using EventMaster.BLL.Exceptions;

namespace AuthService.BLL.Services;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AuthService(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<AuthReponseDTO> RegisterAsync(RegisterDTO registerDto, CancellationToken cancellationToken = default)
    {
        var userFromDb = await _unitOfWork.Users.GetByLoginAsync(registerDto.Login, cancellationToken);
        if (userFromDb != null)
        {
            throw new AlreadyExistsException("User");
        }

        var newUser = _mapper.Map<User>(registerDto);
        newUser.PasswordHash = PasswordHelper.HashPassword(registerDto.Password);
        newUser.CompanyId = null;

        await _unitOfWork.Users.CreateAsync(newUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var role = await _unitOfWork.Roles.GetByNameAsync("Бухгалтер");
        var user = await _unitOfWork.Users.GetByLoginAsync(registerDto.Login);

        await _unitOfWork.Roles.SetRoleToUserAsync(user.Id, role.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var token = _tokenService.GenerateAccessToken(user, new List<Role>() { role });
        return new AuthReponseDTO() { AccessToken = token, UserId = user.Id };
    }

    public async Task<AuthReponseDTO> LoginAsync(LoginDTO loginDto, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByLoginAsync(loginDto.Login, cancellationToken);

        if (user == null)
        {
            throw new EntityNotFoundException($"User with login {loginDto.Login} does not exist");
        }

        var rolesByUser = await _unitOfWork.Roles.GetRolesByUserIdAsync(user.Id, cancellationToken);
        var token = _tokenService.GenerateAccessToken(user, rolesByUser);
        return new AuthReponseDTO() { AccessToken = token, UserId = user.Id };
    }
}