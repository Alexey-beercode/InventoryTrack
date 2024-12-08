using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moq;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.Exceptions;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Application.Services;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Enums;
using WriteOffService.Domain.Interfaces.UnitOfWork;

namespace WriteOffService.Tests.Services;

public class WriteOffRequestServiceTests
{
    [Fact]
    public async Task GetByWarehouseIdAsync_ShouldReturnRequests()
    {
        // Arrange
        var warehouseId = Guid.NewGuid();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockDocumentService = new Mock<IDocumentService>();
        var mockWriteOffReasonService = new Mock<IWriteOffReasonService>();

        var requests = new List<WriteOffRequest>
        {
            new WriteOffRequest { Id = Guid.NewGuid(), WarehouseId = warehouseId, Quantity = 10 }
        };

        mockUnitOfWork
            .Setup(uow => uow.WriteOffRequests.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(requests);

        var service = new WriteOffRequestService(
            mockMapper.Object, 
            mockUnitOfWork.Object, 
            mockDocumentService.Object, 
            mockWriteOffReasonService.Object
        );

        // Act
        var result = await service.GetByWarehouseIdAsync(warehouseId);

        // Assert
        Assert.NotNull(result);
        mockUnitOfWork.Verify(uow => uow.WriteOffRequests.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_ShouldThrowExceptionIfNotFound()
    {
        // Arrange
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockDocumentService = new Mock<IDocumentService>();
        var mockWriteOffReasonService = new Mock<IWriteOffReasonService>();
        var nonExistentId = Guid.NewGuid();

        mockUnitOfWork
            .Setup(uow => uow.WriteOffRequests.GetByIdAsync(nonExistentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((WriteOffRequest)null);

        var service = new WriteOffRequestService(
            mockMapper.Object, 
            mockUnitOfWork.Object, 
            mockDocumentService.Object, 
            mockWriteOffReasonService.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => service.GetByIdAsync(nonExistentId));
    }

    [Fact]
    public async Task DeleteWriteOffRequestAsync_ShouldDeleteRequest()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockDocumentService = new Mock<IDocumentService>();
        var mockWriteOffReasonService = new Mock<IWriteOffReasonService>();

        var request = new WriteOffRequest { Id = requestId };

        mockUnitOfWork
            .Setup(uow => uow.WriteOffRequests.GetByIdAsync(requestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        mockUnitOfWork
            .Setup(uow => uow.WriteOffRequests.Delete(request));

        var service = new WriteOffRequestService(
            mockMapper.Object, 
            mockUnitOfWork.Object, 
            mockDocumentService.Object, 
            mockWriteOffReasonService.Object
        );

        // Act
        await service.DeleteWriteOffRequestAsync(requestId);

        // Assert
        mockUnitOfWork.Verify(uow => uow.WriteOffRequests.Delete(request), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateAsync_ShouldUpdateRequestStatus()
    {
        // Arrange
        var updateDto = new UpdateWriteOffRequestDto
        {
            Id = Guid.NewGuid(),
            Status = RequestStatus.Rejected,
            ApprovedByUserId = Guid.NewGuid()
        };

        var request = new WriteOffRequest
        {
            Id = updateDto.Id,
            Status = RequestStatus.Requested
        };

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockMapper = new Mock<IMapper>();
        var mockDocumentService = new Mock<IDocumentService>();
        var mockWriteOffReasonService = new Mock<IWriteOffReasonService>();

        mockUnitOfWork.Setup(uow => uow.WriteOffRequests.GetByIdAsync(updateDto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var service = new WriteOffRequestService(
            mockMapper.Object,
            mockUnitOfWork.Object,
            mockDocumentService.Object,
            mockWriteOffReasonService.Object
        );

        // Act
        await service.UpdateAsync(updateDto);

        // Assert
        Assert.Equal(RequestStatus.Rejected, request.Status);
        mockUnitOfWork.Verify(uow => uow.WriteOffRequests.Update(request), Times.Once);
        mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}