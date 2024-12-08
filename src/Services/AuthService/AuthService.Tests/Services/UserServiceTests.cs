using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Interfaces.Facades;
using AuthService.BLL.Services;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;
using EventMaster.BLL.Exceptions;
using Moq;

namespace AuthService.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IUserFacade> _mockUserFacade;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockUserFacade = new Mock<IUserFacade>();
            _userService = new UserService(_mockMapper.Object, _mockUnitOfWork.Object, _mockUserFacade.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Login = "user1" },
                new User { Id = Guid.NewGuid(), Login = "user2" }
            };

            var userDtos = new List<UserRepsonseDTO>
            {
                new UserRepsonseDTO { Id = users[0].Id, Login = "user1" },
                new UserRepsonseDTO { Id = users[1].Id, Login = "user2" }
            };

            _mockUnitOfWork.Setup(uow => uow.Users.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
            _mockUserFacade.SetupSequence(uf => uf.GetFullDtoAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userDtos[0])
                .ReturnsAsync(userDtos[1]);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Login = "user1" };
            var userDto = new UserRepsonseDTO { Id = userId, Login = "user1" };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockUserFacade.Setup(uf => uf.GetFullDtoAsync(user, It.IsAny<CancellationToken>())).ReturnsAsync(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowIfUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.GetByIdAsync(userId));
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var updateUserDto = new UpdateUserDTO { Id = userId, FirstName = "Updated", LastName = "User" };
            var user = new User { Id = userId, FirstName = "Old", LastName = "User" };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            await _userService.UpdateAsync(updateUserDto);

            // Assert
            _mockMapper.Verify(m => m.Map(updateUserDto, user), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.Users.Update(user), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowIfUserNotFound()
        {
            // Arrange
            var updateUserDto = new UpdateUserDTO { Id = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(updateUserDto.Id, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.UpdateAsync(updateUserDto));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Users.DeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowIfUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Users.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeleteAsync(userId));
        }
    }
}
