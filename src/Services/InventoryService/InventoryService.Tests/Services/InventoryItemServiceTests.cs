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
        IInventoryItemFacade inventoryItemFacade = new InventoryItemFacade(_unitOfWorkMock.Object,_mapperMock.Object);
        _service = new InventoryItemService(_unitOfWorkMock.Object, _mapperMock.Object, _documentServiceMock.Object,inventoryItemFacade);
    }
    
    [Fact]
    public async Task CreateInventoryItemAsync_ShouldThrowExceptionWhenSupplierNotFound()
    {
        // Arrange
        var createDto = new CreateInventoryItemDto
        {
            Name = "Test Item",
            UniqueCode = "12345",
            Quantity = 10,
            EstimatedValue = 100,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            SupplierId = Guid.NewGuid(), // Non-existent SupplierId
            WarehouseId = Guid.NewGuid(),
            DeliveryDate = DateTime.UtcNow,
            DocumentFile = new Mock<IFormFile>().Object
        };

        _unitOfWorkMock.Setup(uow => uow.Suppliers.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Supplier)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.CreateInventoryItemAsync(createDto));
    }
    
    [Fact]
    public async Task CreateInventoryItemAsync_ShouldCreateItem()
    {
        // Arrange
        var createDto = new CreateInventoryItemDto
        {
            Name = "Test Item",
            UniqueCode = "12345",
            Quantity = 10,
            EstimatedValue = 100,
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            SupplierId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            DeliveryDate = DateTime.UtcNow,
            DocumentFile = new Mock<IFormFile>().Object // Mocked IFormFile
        };

        var document = new Document { Id = Guid.NewGuid() };
        var inventoryItem = new InventoryItem { Id = Guid.NewGuid(), Name = "Test Item" };

        _unitOfWorkMock.Setup(uow => uow.Suppliers.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Supplier { Id = Guid.NewGuid(), Name = "Test Supplier" ,AccountNumber = "crvgbvfcds",PhoneNumber = "sdervtbgtvrcxs",PostalAddress = "ecrvtgbtvrewefrtv"});

        _documentServiceMock.Setup(ds => ds.CreateDocumentAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Document { Id = Guid.NewGuid(), FileName = "Test.pdf", FileType = "pdf", FileData = new byte[] { 1, 2, 3 } });


        _mapperMock.Setup(m => m.Map<InventoryItem>(createDto)).Returns(inventoryItem);

        // Act
        await _service.CreateInventoryItemAsync(createDto);

        // Assert
        _documentServiceMock.Verify(ds => ds.CreateDocumentAsync(createDto.DocumentFile, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.InventoryItems.CreateAsync(inventoryItem, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2)); // Once for InventoryItem and once for InventoriesItemsWarehouses
    }

    [Fact]
    public async Task GetInventoryItemAsync_ShouldReturnItem()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var warehouses = new List<InventoriesItemsWarehouses>
        {
            new InventoriesItemsWarehouses
            {
                WarehouseId = Guid.NewGuid(),
                ItemId = itemId,
                Quantity = 5,
                InventoryItem = new InventoryItem { Id = itemId, Name = "Item 1" },
                Warehouse = new Warehouse { Name = "Warehouse 1" }
            }
        };

        _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByItemIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouses);

        // Act
        var result = await _service.GetInventoryItemAsync(itemId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(warehouses.Sum(w => w.Quantity), result.Quantity);
    }

    [Fact]
    public async Task GetByNameAsync_ShouldReturnItem()
    {
        // Arrange
        var name = "Item 1";
        var inventoryItem = new InventoryItem { Id = Guid.NewGuid(), Name = name };
        var warehouses = new List<InventoriesItemsWarehouses>
        {
            new InventoriesItemsWarehouses
            {
                ItemId = inventoryItem.Id,
                Quantity = 10,
                Warehouse = new Warehouse { Name = "Warehouse 1" }
            }
        };

        _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByNameAsync(name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByItemIdAsync(inventoryItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(warehouses);

        _mapperMock.Setup(m => m.Map<InventoryItemResponseDto>(inventoryItem))
            .Returns(new InventoryItemResponseDto
            {
                Id = inventoryItem.Id,
                Name = inventoryItem.Name,
                Quantity = 10,
                WarehouseDetails = warehouses.Select(w => new WarehouseDetailsDto
                {
                    WarehouseId = w.WarehouseId,
                    WarehouseName = w.Warehouse.Name,
                    Quantity = w.Quantity
                }).ToList()
            });

        // Act
        var result = await _service.GetByNameAsync(name);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Quantity);
    }

    [Fact]
    public async Task UpdateInventoryItemStatusAsync_ShouldDeleteItemWhenRejected()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        var inventoryItem = new InventoryItem { Id = itemId, Status = InventoryItemStatus.Created };
        var updateDto = new ChangeInventoryItemStatusDto
        {
            InventoryItemId = itemId,
            Status = InventoryItemStatus.Rejected
        };

        _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryItem);

        _unitOfWorkMock.Setup(uow => uow.InventoryItems.Delete(It.IsAny<InventoryItem>()));
        _documentServiceMock.Setup(ds => ds.DeleteDocumentAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()));

        // Act
        await _service.UpdateInventoryItemStatusAsync(updateDto);

        // Assert
        _unitOfWorkMock.Verify(uow => uow.InventoryItems.Delete(inventoryItem), Times.Once);
        _documentServiceMock.Verify(ds => ds.DeleteDocumentAsync(inventoryItem.DocumentId, It.IsAny<CancellationToken>()), Times.Once);
    }

    // Test 7: Move item
    [Fact]
    public async Task MoveItemAsync_ShouldMoveItem()
    {
        // Arrange
        var sourceWarehouseId = Guid.NewGuid();
        var destinationWarehouseId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        var sourceEntry = new InventoriesItemsWarehouses { WarehouseId = sourceWarehouseId, ItemId = itemId, Quantity = 10 };
        _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(sourceWarehouseId, It.IsAny<CancellationToken>()))
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
        _unitOfWorkMock.Verify(uow => uow.InventoriesItemsWarehouses.Update(It.IsAny<InventoriesItemsWarehouses>()), Times.AtLeastOnce);
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

    _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<InventoriesItemsWarehouses> { entry });

    // Act
    await _service.WriteOffItemAsync(itemId, warehouseId, writeOffQuantity);

    // Assert
    Assert.Equal(initialQuantity - writeOffQuantity, entry.Quantity);
    _unitOfWorkMock.Verify(uow => uow.InventoriesItemsWarehouses.Update(entry), Times.Once);
    _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}

// Test 9: Get inventory items by status
[Fact]
public async Task GetByStatusAsync_ShouldReturnItemsByStatus()
{
    // Arrange
    var status = InventoryItemStatus.Created;
    var items = new List<InventoryItem>
    {
        new InventoryItem { Id = Guid.NewGuid(), Status = status, Name = "Item 1" },
        new InventoryItem { Id = Guid.NewGuid(), Status = status, Name = "Item 2" }
    };

    _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByStatusAsync(status, It.IsAny<CancellationToken>()))
        .ReturnsAsync(items);

    _mapperMock.Setup(m => m.Map<InventoryItemResponseDto>(It.IsAny<InventoryItem>()))
        .Returns((InventoryItem item) => new InventoryItemResponseDto { Id = item.Id, Name = item.Name });

    // Act
    var result = await _service.GetByStatusAsync(status);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(items.Count, result.Count());
    _unitOfWorkMock.Verify(uow => uow.InventoryItems.GetByStatusAsync(status, It.IsAny<CancellationToken>()), Times.Once);
}

// Test 10: Update inventory item
[Fact]
public async Task UpdateInventoryItemAsync_ShouldUpdateItem()
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
    var item = new InventoryItem { Id = itemId, Name = "Old Name", Status = InventoryItemStatus.Created };

    _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(item);

    // Act
    await _service.UpdateInventoryItemAsync(itemId, updateDto);

    // Assert
    Assert.Equal(updateDto.Name, item.Name);
    Assert.Equal(updateDto.Status, item.Status);
    _unitOfWorkMock.Verify(uow => uow.InventoryItems.Update(item), Times.Once);
    _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
}

// Test 11: Get inventory items by warehouse
[Fact]
public async Task GetInventoryItemsByWarehouseAsync_ShouldReturnItemsInWarehouse()
{
    // Arrange
    var warehouseId = Guid.NewGuid();
    var itemsWarehouses = new List<InventoriesItemsWarehouses>
    {
        new InventoriesItemsWarehouses
        {
            WarehouseId = warehouseId,
            ItemId = Guid.NewGuid(),
            Quantity = 10,
            InventoryItem = new InventoryItem { Id = Guid.NewGuid(), Name = "Item 1" },
            Warehouse = new Warehouse { Name = "Warehouse 1" }
        }
    };

    _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(itemsWarehouses);

    // Act
    var result = await _service.GetInventoryItemsByWarehouseAsync(warehouseId);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal(warehouseId, result.First().WarehouseDetails.First().WarehouseId);
}

// Test 12: Error handling for non-existing item in GetByNameAsync
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

// Test 13: Validate exception on creating an item with invalid document
    [Fact]
    public async Task CreateInventoryItemAsync_ShouldThrowExceptionForInvalidDocument()
    {
        // Arrange
        var createDto = new CreateInventoryItemDto
        {
            Name = "Invalid Item",
            UniqueCode = "67890",
            Quantity = 5,
            EstimatedValue = 50,
            ExpirationDate = DateTime.UtcNow.AddDays(20),
            SupplierId = Guid.NewGuid(),
            WarehouseId = Guid.NewGuid(),
            DeliveryDate = DateTime.UtcNow,
            DocumentFile = new Mock<IFormFile>().Object // Mocked IFormFile
        };

        _documentServiceMock.Setup(ds => ds.CreateDocumentAsync(createDto.DocumentFile, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Document)null); // Simulate invalid document

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateInventoryItemAsync(createDto));

        // Verify interactions
        _documentServiceMock.Verify(ds => ds.CreateDocumentAsync(createDto.DocumentFile, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(uow => uow.InventoryItems.CreateAsync(It.IsAny<InventoryItem>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }


// Test 14: Validate exception on moving an item with insufficient quantity
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

    _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByWarehouseIdAsync(sourceWarehouseId, It.IsAny<CancellationToken>()))
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

    _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetFilteredItemsAsync(filterDto.Name, null, null, null, default))
        .ReturnsAsync(new List<InventoryItem>());

    // Act
    var result = await _service.GetFilteredItemsAsync(filterDto);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
    _unitOfWorkMock.Verify(uow => uow.InventoryItems.GetFilteredItemsAsync(filterDto.Name, null, null, null, default), Times.Once);
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

// Test 17: Get all inventory items when no items exist
[Fact]
public async Task GetInventoryItemsAsync_ShouldReturnEmptyListWhenNoItemsExist()
{
    // Arrange
    _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetAllAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<InventoriesItemsWarehouses>());

    // Act
    var result = await _service.GetInventoryItemsAsync();

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}

// Test 18: Validate exception on retrieving a non-existing inventory item by ID
[Fact]
public async Task GetInventoryItemAsync_ShouldThrowExceptionForNonExistingItem()
{
    // Arrange
    var nonExistentItemId = Guid.NewGuid();
    _unitOfWorkMock.Setup(uow => uow.InventoriesItemsWarehouses.GetByItemIdAsync(nonExistentItemId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(new List<InventoriesItemsWarehouses>());

    // Act & Assert
    await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetInventoryItemAsync(nonExistentItemId));
}

// Test 19: Validate exception when moving an item to the same warehouse
[Fact]
public async Task MoveItemAsync_ShouldThrowExceptionForSameSourceAndDestination()
{
    // Arrange
    var warehouseId = Guid.NewGuid();
    var itemId = Guid.NewGuid();

    var moveItemDto = new MoveItemDto
    {
        SourceWarehouseId = warehouseId,
        DestinationWarehouseId = warehouseId,
        ItemId = itemId,
        Quantity = 5
    };

    // Act & Assert
    var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.MoveItemAsync(moveItemDto));
    Assert.Equal("Source and destination warehouses cannot be the same.", exception.Message);
}

// Test 20: Validate creating an inventory item with a missing document
[Fact]
public async Task CreateInventoryItemAsync_ShouldThrowExceptionForMissingDocument()
{
    // Arrange
    var createDto = new CreateInventoryItemDto
    {
        Name = "Item without Document",
        UniqueCode = "0001",
        Quantity = 5,
        EstimatedValue = 50,
        ExpirationDate = DateTime.UtcNow.AddDays(10),
        SupplierId = Guid.NewGuid(),
        WarehouseId = Guid.NewGuid(),
        DeliveryDate = DateTime.UtcNow,
        DocumentFile = null // Missing document
    };

    _documentServiceMock.Setup(ds => ds.CreateDocumentAsync(It.IsAny<IFormFile>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync((Document)null);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateInventoryItemAsync(createDto));
}

// Test 21: Validate deleting an inventory item that is already deleted
[Fact]
public async Task DeleteInventoryItemAsync_ShouldThrowExceptionForAlreadyDeletedItem()
{
    // Arrange
    var itemId = Guid.NewGuid();
    var inventoryItem = new InventoryItem { Id = itemId, IsDeleted = true };

    _unitOfWorkMock.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(inventoryItem);

    // Act & Assert
    await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteInventoryItemAsync(itemId));
}
}