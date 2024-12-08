using InventoryService.Application.DTOs.Request.Supplier;
using InventoryService.Application.DTOs.Response.Supplier;

namespace InventoryService.Application.Interfaces.Services;

public interface ISupplierService
{
    Task<SupplierResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<SupplierResponseDto> GetByAccountNumber(string accountNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<SupplierResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SupplierResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateSupplierDto createSupplierDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}