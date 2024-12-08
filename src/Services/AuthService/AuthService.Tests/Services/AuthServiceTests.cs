using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.Exceptions;
using AuthService.BLL.Helpers;
using AuthService.BLL.Interfaces.Services;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;
using EventMaster.BLL.Exceptions;
using Moq;

namespace AuthService.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService.BLL.Services.AuthService _authService;

        public AuthServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _authService = new AuthService.BLL.Services.AuthService(_mockMapper.Object, _mockUnitOfWork.Object, _mockTokenService.Object);
        }
        
        [Fact]
        public async Task RegisterAsync_ShouldThrowExceptionIfUserAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDTO { Login = "johndoe" };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByLoginAsync(registerDto.Login, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => _authService.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTokenForValidUser()
        {
            // Arrange
            var loginDto = new LoginDTO { Login = "johndoe", Password = "Password123" };

            var user = new User { Id = Guid.NewGuid(), Login = "johndoe", PasswordHash = PasswordHelper.HashPassword("Password123") };
            var roles = new List<Role> { new Role { Name = "Resident" } };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByLoginAsync(loginDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            _mockTokenService.Setup(ts => ts.GenerateAccessToken(user, roles)).Returns("mock-token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("mock-token", result.AccessToken);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowExceptionIfUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDTO { Login = "nonexistentuser" };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByLoginAsync(loginDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _authService.LoginAsync(loginDto));
        }
        
        [Fact]
        public async Task LoginAsync_ShouldIncludeRolesInToken()
        {
            // Arrange
            var loginDto = new LoginDTO { Login = "johndoe" };

            var user = new User { Id = Guid.NewGuid(), Login = "johndoe" };
            var roles = new List<Role>
            {
                new Role { Name = "Resident" },
                new Role { Name = "Accountant" }
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByLoginAsync(loginDto.Login, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockUnitOfWork.Setup(uow => uow.Roles.GetRolesByUserIdAsync(user.Id, It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            _mockTokenService.Setup(ts => ts.GenerateAccessToken(user, roles)).Returns("mock-token");

            // Act
            var result = await _authService.LoginAsync(loginDto);

            // Assert
            Assert.Equal("mock-token", result.AccessToken);
        }
        
    }
}
