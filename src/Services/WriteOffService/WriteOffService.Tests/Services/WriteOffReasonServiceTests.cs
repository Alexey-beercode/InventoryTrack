using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using WriteOffService.Application.DTOs.Response.WriteOffReason;
using WriteOffService.Application.Services;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Interfaces.UnitOfWork;
using Xunit;

namespace WriteOffService.Tests.Services
{
    public class WriteOffReasonServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly WriteOffReasonService _service;

        public WriteOffReasonServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new WriteOffReasonService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllReasons()
        {
            // Arrange
            var reasons = new List<WriteOffReason>
            {
                new WriteOffReason { Id = Guid.NewGuid(), Reason = "Reason 1" },
                new WriteOffReason { Id = Guid.NewGuid(), Reason = "Reason 2" }
            };

            var reasonDtos = reasons.Select(r => new WriteOffReasonResponseDto
            {
                Id = r.Id,
                Reason = r.Reason
            });

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(reasons);

            _mockMapper.Setup(m => m.Map<IEnumerable<WriteOffReasonResponseDto>>(reasons))
                       .Returns(reasonDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnReasonById()
        {
            // Arrange
            var reasonId = Guid.NewGuid();
            var reason = new WriteOffReason { Id = reasonId, Reason = "Test Reason" };

            var reasonDto = new WriteOffReasonResponseDto { Id = reasonId, Reason = "Test Reason" };

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.GetByIdAsync(reasonId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(reason);

            _mockMapper.Setup(m => m.Map<WriteOffReasonResponseDto>(reason))
                       .Returns(reasonDto);

            // Act
            var result = await _service.GetByIdAsync(reasonId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reasonDto.Id, result.Id);
            Assert.Equal(reasonDto.Reason, result.Reason);
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.GetByIdAsync(reasonId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNullIfNotFound()
        {
            // Arrange
            var reasonId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.GetByIdAsync(reasonId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((WriteOffReason)null);

            // Act
            var result = await _service.GetByIdAsync(reasonId);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.GetByIdAsync(reasonId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnReasonByName()
        {
            // Arrange
            var name = "Test Reason";
            var reason = new WriteOffReason { Id = Guid.NewGuid(), Reason = name };

            var reasonDto = new WriteOffReasonResponseDto { Id = reason.Id, Reason = name };

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(reason);

            _mockMapper.Setup(m => m.Map<WriteOffReasonResponseDto>(reason))
                       .Returns(reasonDto);

            // Act
            var result = await _service.GetByNameAsync(name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(reasonDto.Id, result.Id);
            Assert.Equal(reasonDto.Reason, result.Reason);
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.GetByNameAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnNullIfNotFound()
        {
            // Arrange
            var name = "Nonexistent Reason";

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((WriteOffReason)null);

            // Act
            var result = await _service.GetByNameAsync(name);

            // Assert
            Assert.Null(result);
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.GetByNameAsync(name, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateReason()
        {
            // Arrange
            var name = "New Reason";

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.CreateAsync(It.IsAny<WriteOffReason>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);
            

            // Act
            await _service.CreateAsync(name);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.CreateAsync(It.Is<WriteOffReason>(r => r.Reason == name), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldNotThrowExceptionOnEmptyName()
        {
            // Arrange
            var name = string.Empty;

            _mockUnitOfWork.Setup(uow => uow.WriteOffReasons.CreateAsync(It.IsAny<WriteOffReason>(), It.IsAny<CancellationToken>()))
                           .Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync(name);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.WriteOffReasons.CreateAsync(It.IsAny<WriteOffReason>(), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
