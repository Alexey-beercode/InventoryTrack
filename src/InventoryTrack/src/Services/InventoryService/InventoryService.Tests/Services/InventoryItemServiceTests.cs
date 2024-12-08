using AutoMapper;
using BookingService.Application.Exceptions;
using Moq;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Application.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Tests.Services
{
    public class InventoryItemServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IDocumentService> _mockDocumentService;
        private readonly InventoryItemService _service;

        public InventoryItemServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockDocumentService = new Mock<IDocumentService>();
            _service = new InventoryItemService(_mockUnitOfWork.Object, _mockMapper.Object, _mockDocumentService.Object);
        }

        [Fact]
        public async Task GetInventoryItemAsync_ShouldReturnItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var inventoryItem = new InventoryItem { Id = itemId, Name = "Test Item" };

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(inventoryItem);

            _mockMapper.Setup(m => m.Map<InventoryItemResponseDto>(inventoryItem))
                       .Returns(new InventoryItemResponseDto { Id = itemId, Name = "Test Item" });

            // Act
            var result = await _service.GetInventoryItemAsync(itemId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(itemId, result.Id);
            _mockUnitOfWork.Verify(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInventoryItemAsync_ShouldThrowIfNotFound()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((InventoryItem)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetInventoryItemAsync(itemId));
        }

        [Fact]
        public async Task DeleteInventoryItemAsync_ShouldDeleteItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var inventoryItem = new InventoryItem { Id = itemId, DocumentId = Guid.NewGuid() };

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(inventoryItem);

            // Act
            await _service.DeleteInventoryItemAsync(itemId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.InventoryItems.Delete(inventoryItem), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _mockDocumentService.Verify(ds => ds.DeleteDocumentAsync(inventoryItem.DocumentId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetInventoryItemsByWarehouseAsync_ShouldReturnItems()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var items = new List<InventoryItem>
            {
                new InventoryItem { Id = Guid.NewGuid(), WarehouseId = warehouseId, Name = "Item 1" },
                new InventoryItem { Id = Guid.NewGuid(), WarehouseId = warehouseId, Name = "Item 2" }
            };

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByWarehouseIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(items);

            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryItemResponseDto>>(items))
                       .Returns(items.Select(i => new InventoryItemResponseDto { Id = i.Id, Name = i.Name }));

            // Act
            var result = await _service.GetInventoryItemsByWarehouseAsync(warehouseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByUniqueCodeAsync_ShouldReturnItem()
        {
            // Arrange
            var uniqueCode = "ABC123";
            var item = new InventoryItem { Id = Guid.NewGuid(), UniqueCode = uniqueCode };

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByUniqueCodeAsync(uniqueCode, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(item);

            _mockMapper.Setup(m => m.Map<InventoryItemResponseDto>(item))
                       .Returns(new InventoryItemResponseDto { Id = item.Id, UniqueCode = uniqueCode });

            // Act
            var result = await _service.GetByUniqueCodeAsync(uniqueCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(uniqueCode, result.UniqueCode);
        }

        [Fact]
        public async Task UpdateInventoryItemAsync_ShouldUpdateItem()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = new InventoryItem { Id = itemId, Name = "Old Name" };
            var updateDto = new UpdateInventoryItemDto {Name = "New name"};

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByIdAsync(itemId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(item);

            // Act
            await _service.UpdateInventoryItemAsync(itemId, updateDto);

            // Assert
            _mockMapper.Verify(m => m.Map(updateDto, item), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.InventoryItems.Update(item), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnItems()
        {
            // Arrange
            var status = InventoryItemStatus.Created;
            var items = new List<InventoryItem>
            {
                new InventoryItem { Id = Guid.NewGuid(), Status = status },
                new InventoryItem { Id = Guid.NewGuid(), Status = status }
            };

            _mockUnitOfWork.Setup(uow => uow.InventoryItems.GetByStatusAsync(status, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(items);

            _mockMapper.Setup(m => m.Map<IEnumerable<InventoryItemResponseDto>>(items))
                       .Returns(items.Select(i => new InventoryItemResponseDto { Id = i.Id }));

            // Act
            var result = await _service.GetByStatusAsync(status);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
