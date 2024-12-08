using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Facades;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Application.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Moq;

namespace InventoryService.Tests.Services;

public class InventoryItemServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IDocumentService> _documentServiceMock;
    private readonly InventoryItemService _service;

    public InventoryItemServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _documentServiceMock = new Mock<IDocumentService>();
        IInventoryItemFacade inventoryItemFacade = new InventoryItemFacade(_unitOfWorkMock.Object, _mapperMock.Object);
        _service = new InventoryItemService(_unitOfWorkMock.Object, _mapperMock.Object, _documentServiceMock.Object,
            inventoryItemFacade);
    }

    [Fact]
    public async Task MoveItemAsync_ShouldMoveItem()
    {
        // Arrange
        var sourceWarehouseId = Guid.NewGuid();
        var destinationWarehouseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var sourceEntry = new InventoriesItemsWarehouses
            { WarehouseId = sourceWarehouseId, ItemId = itemId, Quantity = 10 };
        _unitOfWorkMock.Setup(uow =>
                uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(sourceWarehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoriesItemsWarehouses> { sourceEntry });

        // Act
        await _service.MoveItemAsync(new MoveItemDto
        {
            SourceWarehouseId = sourceWarehouseId,
            DestinationWarehouseId = destinationWarehouseId,
            ItemId = itemId,
            Quantity = 5
        });

        // Assert
        Assert.Equal(5, sourceEntry.Quantity);
        _unitOfWorkMock.Verify(uow => uow.InventoriesItemsWarehouses.Update(It.IsAny<InventoriesItemsWarehouses>()),
            Times.AtLeastOnce);
    }

// Test 8: Write-off inventory item
    [Fact]
    public async Task WriteOffItemAsync_ShouldWriteOffCorrectly()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var initialQuantity = 10;
        var writeOffQuantity = 5;

        var entry = new InventoriesItemsWarehouses
        {
            WarehouseId = warehouseId,
            ItemId = itemId,
            Quantity = initialQuantity
        };

        _unitOfWorkMock.Setup(uow =>
                uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoriesItemsWarehouses> { entry });

        // Act
        await _service.WriteOffItemAsync(itemId, warehouseId, writeOffQuantity);

        // Assert
        Assert.Equal(initialQuantity - writeOffQuantity, entry.Quantity);
        _unitOfWorkMock.Verify(uow => uow.InventoriesItemsWarehouses.Update(entry), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldThrowExceptionForNonExistingItem()
    {
        // Arrange
        var name = "NonExistingItem";
        _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByNameAsync(name));
    }

    [Fact]
    public async Task MoveItemAsync_ShouldThrowExceptionForInsufficientQuantity()
    {
        // Arrange
        var sourceWarehouseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var sourceEntry = new InventoriesItemsWarehouses
        {
            WarehouseId = sourceWarehouseId,
            ItemId = itemId,
            Quantity = 5
        };

        _unitOfWorkMock.Setup(uow =>
                uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(sourceWarehouseId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoriesItemsWarehouses> { sourceEntry });

        var moveItemDto = new MoveItemDto
        {
            SourceWarehouseId = sourceWarehouseId,
            DestinationWarehouseId = Guid.NewGuid(),
            ItemId = itemId,
            Quantity = 10
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MoveItemAsync(moveItemDto));
    }

// Test 15: Validate response when no items match filter criteria
    [Fact]
    public async Task GetFilteredItemsAsync_ShouldReturnEmptyListWhenNoMatches()
    {
        // Arrange
        var filterDto = new FilterInventoryItemDto
        {
            Name = "NonExistingItem"
        };

        _unitOfWorkMock
            .Setup(uow => uow.InventoryItems.GetFilteredItemsAsync(filterDto.Name, null, null, null, default))
            .ReturnsAsync(new List<InventoryItem>());

        // Act
        var result = await _service.GetFilteredItemsAsync(filterDto);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _unitOfWorkMock.Verify(
            uow => uow.InventoryItems.GetFilteredItemsAsync(filterDto.Name, null, null, null, default), Times.Once);
    }

// Test 16: Validate exception on updating a non-existing item
    [Fact]
    public async Task UpdateInventoryItemAsync_ShouldThrowExceptionForNonExistingItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var updateDto = new UpdateInventoryItemDto
        {
            Name = "Updated Name",
            Quantity = 15,
            WarehouseId = Guid.NewGuid(),
            Status = InventoryItemStatus.Requested
        };

        _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((InventoryItem)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateInventoryItemAsync(itemId, updateDto));
    }
}
