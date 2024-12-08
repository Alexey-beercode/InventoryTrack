using AutoMapper;
using BookingService.Application.Exceptions;
using Moq;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.DTOs.Response.WarehouseType;
using InventoryService.Application.Facades;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Tests.Services
{
    public class WarehouseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly WarehouseService _service;

        public WarehouseServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            IInventoryItemFacade inventoryItemFacade = new InventoryItemFacade(_mockUnitOfWork.Object,_mockMapper.Object);
            _service = new WarehouseService(_mockMapper.Object, _mockUnitOfWork.Object,inventoryItemFacade);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllWarehouses()
        {
            // Arrange
            var warehouses = new List<Warehouse> { new Warehouse { Id = Guid.NewGuid(), Name = "Warehouse1" } };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetAllAsync(It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouses);

            var warehouseDtos = warehouses.Select(w => new WarehouseResponseDto { Id = w.Id, Name = w.Name });

            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseResponseDto>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            _mockUnitOfWork.Verify(uow => uow.Warehouses.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnWarehouse()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var warehouse = new Warehouse { Id = warehouseId, Name = "Warehouse1" };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouse);

            var warehouseDto = new WarehouseResponseDto { Id = warehouse.Id, Name = warehouse.Name };

            _mockMapper.Setup(m => m.Map<WarehouseResponseDto>(warehouse)).Returns(warehouseDto);

            // Act
            var result = await _service.GetByIdAsync(warehouseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(warehouseId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowIfNotFound()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByIdAsync(warehouseId));
        }

        [Fact]
        public async Task GetByTypeAsync_ShouldReturnWarehousesByType()
        {
            // Arrange
            var warehouseType = WarehouseType.Production;
            var warehouses = new List<Warehouse> { new Warehouse { Id = Guid.NewGuid(), Type = warehouseType } };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByTypeAsync(warehouseType, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouses);

            var warehouseDtos = warehouses.Select(w => new WarehouseResponseDto { Id = w.Id, Type = new WarehouseTypeResponseDto(){Name = "Production",Value = WarehouseType.Production}});

            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseResponseDto>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _service.GetByTypeAsync(warehouseType);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByCompanyIdAsync_ShouldReturnWarehouses()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var warehouses = new List<Warehouse> { new Warehouse { Id = Guid.NewGuid(), CompanyId = companyId } };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByCompanyIdAsync(companyId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouses);

            var warehouseDtos = warehouses.Select(w => new WarehouseResponseDto { Id = w.Id, CompanyId = w.CompanyId });

            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseResponseDto>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _service.GetByCompanyIdAsync(companyId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByResponsiblePersonIdAsync_ShouldReturnWarehouses()
        {
            // Arrange
            var personId = Guid.NewGuid();
            var warehouses = new List<Warehouse> { new Warehouse { Id = Guid.NewGuid(), ResponsiblePersonId = personId } };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByResponsiblePersonIdAsync(personId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouses);

            var warehouseDtos = warehouses.Select(w => new WarehouseResponseDto { Id = w.Id, ResponsiblePersonId = w.ResponsiblePersonId });

            _mockMapper.Setup(m => m.Map<IEnumerable<WarehouseResponseDto>>(warehouses)).Returns(warehouseDtos);

            // Act
            var result = await _service.GetByResponsiblePersonIdAsync(personId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteWarehouse()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();
            var warehouse = new Warehouse { Id = warehouseId };

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(warehouse);

            // Act
            await _service.DeleteAsync(warehouseId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Warehouses.Delete(warehouse), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowIfNotFound()
        {
            // Arrange
            var warehouseId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Warehouses.GetByIdAsync(warehouseId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Warehouse)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(warehouseId));
        }
    }
}
