using AutoMapper;
using BookingService.Application.Exceptions;
using Moq;
using InventoryService.Application.DTOs.Request.Supplier;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Application.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Tests.Services
{
    public class SupplierServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _service = new SupplierService(_mockMapper.Object, _mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnSupplier()
        {
            // Arrange
            var name = "SupplierName";
            var supplier = new Supplier { Id = Guid.NewGuid(), Name = name };

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(supplier);

            _mockMapper.Setup(m => m.Map<SupplierResponseDto>(supplier))
                       .Returns(new SupplierResponseDto { Id = supplier.Id, Name = supplier.Name });

            // Act
            var result = await _service.GetByNameAsync(name);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(name, result.Name);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldThrowIfNotFound()
        {
            // Arrange
            var name = "NonExistentSupplier";

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByNameAsync(name, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByNameAsync(name));
        }

        [Fact]
        public async Task GetByAccountNumber_ShouldReturnSupplier()
        {
            // Arrange
            var accountNumber = "123456789";
            var supplier = new Supplier { Id = Guid.NewGuid(), AccountNumber = accountNumber };

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByAccountNumber(accountNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(supplier);

            _mockMapper.Setup(m => m.Map<SupplierResponseDto>(supplier))
                       .Returns(new SupplierResponseDto { Id = supplier.Id, AccountNumber = supplier.AccountNumber });

            // Act
            var result = await _service.GetByAccountNumber(accountNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(accountNumber, result.AccountNumber);
        }

        [Fact]
        public async Task GetByAccountNumber_ShouldThrowIfNotFound()
        {
            // Arrange
            var accountNumber = "NonExistentAccount";

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByAccountNumber(accountNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.GetByAccountNumber(accountNumber));
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateSupplier()
        {
            // Arrange
            var createDto = new CreateSupplierDto
            {
                Name = "New Supplier",
                AccountNumber = "987654321"
            };

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByAccountNumber(createDto.AccountNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Supplier)null);

            var supplier = new Supplier { Id = Guid.NewGuid(), Name = createDto.Name };

            _mockMapper.Setup(m => m.Map<Supplier>(createDto)).Returns(supplier);

            // Act
            await _service.CreateAsync(createDto);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Suppliers.CreateAsync(It.Is<Supplier>(s => s.Name == createDto.Name), It.IsAny<CancellationToken>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowIfAccountNumberExists()
        {
            // Arrange
            var createDto = new CreateSupplierDto
            {
                Name = "New Supplier",
                AccountNumber = "ExistingAccountNumber"
            };

            var existingSupplier = new Supplier { Id = Guid.NewGuid(), AccountNumber = createDto.AccountNumber };

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByAccountNumber(createDto.AccountNumber, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(existingSupplier);

            // Act & Assert
            await Assert.ThrowsAsync<AlreadyExistsException>(() => _service.CreateAsync(createDto));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteSupplier()
        {
            // Arrange
            var supplierId = Guid.NewGuid();
            var supplier = new Supplier { Id = supplierId };

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(supplier);

            // Act
            await _service.DeleteAsync(supplierId);

            // Assert
            _mockUnitOfWork.Verify(uow => uow.Suppliers.Delete(supplier), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowIfNotFound()
        {
            // Arrange
            var supplierId = Guid.NewGuid();

            _mockUnitOfWork.Setup(uow => uow.Suppliers.GetByIdAsync(supplierId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync((Supplier)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(supplierId));
        }
    }
}
