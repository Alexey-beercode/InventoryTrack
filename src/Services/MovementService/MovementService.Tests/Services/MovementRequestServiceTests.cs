using AutoMapper;
using BookingService.Application.Exceptions;
using MassTransit;
using Moq;
using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Application.Services;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;
using MovementService.Domain.Interfaces.UnitOfWork;
using MovementService.Infrastructure.Messaging.Producers;

namespace MovementService.Tests.Services
{
    public class MovementRequestServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MovementRequestService _service;

        public MovementRequestServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            Mock<IPublishEndpoint> mockPublishEndpoint = new();
            Mock<MovementRequestProducer> mockProducer = new(mockPublishEndpoint.Object);
            _service = new MovementRequestService(_mockUnitOfWork.Object, _mockMapper.Object,mockProducer.Object);
        }
        
        [Fact]
        public async Task RejectMovementRequestAsync_ShouldRejectRequest()
        {
            // Arrange
            var changeStatusDto = new ChangeStatusDto
            {
                RequestId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var movementRequest = new MovementRequest
            {
                Id = changeStatusDto.RequestId,
                Status = MovementRequestStatus.Processing
            };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByIdAsync(changeStatusDto.RequestId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequest);

            // Act
            await _service.RejectMovementRequestAsync(changeStatusDto);

            // Assert
            Assert.Equal(MovementRequestStatus.Rejected, movementRequest.Status);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RejectMovementRequestAsync_ShouldThrowIfRequestNotFound()
        {
            // Arrange
            var changeStatusDto = new ChangeStatusDto
            {
                RequestId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByIdAsync(changeStatusDto.RequestId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((MovementRequest)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.RejectMovementRequestAsync(changeStatusDto));
        }

        [Fact]
        public async Task CompleteMovementRequestAsync_ShouldCompleteRequest()
        {
            // Arrange
            var changeStatusDto = new ChangeStatusDto
            {
                RequestId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            var movementRequest = new MovementRequest
            {
                Id = changeStatusDto.RequestId,
                Status = MovementRequestStatus.Processing
            };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByIdAsync(changeStatusDto.RequestId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequest);

            // Act
            await _service.CompleteMovementRequestAsync(changeStatusDto);

            // Assert
            Assert.Equal(MovementRequestStatus.Completed, movementRequest.Status);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetMovementRequestByIdAsync_ShouldReturnMovementRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var movementRequest = new MovementRequest { Id = requestId };
            var responseDto = new MovementRequestResponseDto { Id = requestId };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequest);

            _mockMapper.Setup(m => m.Map<MovementRequestResponseDto>(movementRequest)).Returns(responseDto);

            // Act
            var result = await _service.GetMovementRequestByIdAsync(requestId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(requestId, result.Id);
        }

        [Fact]
        public async Task GetMovementRequestsByItemIdAsync_ShouldReturnRequests()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var movementRequests = new List<MovementRequest> { new MovementRequest { Id = Guid.NewGuid() } };
            var responseDtos = new List<MovementRequestResponseDto> { new MovementRequestResponseDto { Id = Guid.NewGuid() } };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByItemIdAsync(itemId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequests);

            _mockMapper.Setup(m => m.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests)).Returns(responseDtos);

            // Act
            var result = await _service.GetMovementRequestsByItemIdAsync(itemId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetMovementRequestsBySourceWarehouseIdAsync_ShouldReturnRequests()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var movementRequests = new List<MovementRequest> { new MovementRequest { Id = Guid.NewGuid() } };
            var responseDtos = new List<MovementRequestResponseDto> { new MovementRequestResponseDto { Id = Guid.NewGuid() } };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetBySourceWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequests);

            _mockMapper.Setup(m => m.Map<IEnumerable<MovementRequestResponseDto>>(movementRequests)).Returns(responseDtos);

            // Act
            var result = await _service.GetMovementRequestsBySourceWarehouseIdAsync(warehouseId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task DeleteMovementRequestAsync_ShouldDeleteRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var movementRequest = new MovementRequest { Id = requestId };

            _mockUnitOfWork.Setup(uow => uow.MovementRequests.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(movementRequest);

            // Act
            await _service.DeleteMovementRequestAsync(requestId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.MovementRequests.Delete(movementRequest), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
