using InventoryService.Application.DTOs.Request.Warehouse;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.Interfaces.Services;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<WarehouseResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<WarehouseResponseDto>> GetByTypeAsync(WarehouseType warehouseType,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WarehouseResponseDto>> GetByCompanyIdAsync(Guid companyId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WarehouseResponseDto>> GetByResponsiblePersonIdAsync(Guid responsiblePersonId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WarehouseResponseDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    Task<IEnumerable<WarehouseResponseDto>> GetByLocationAsync(string location,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<WarehouseStateResponseDto>> GetAllWarehousesStateAsync(
        CancellationToken cancellationToken = default);

    Task<WarehouseStateResponseDto> GetStateByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<WarehouseStateResponseDto>> GetWarehousesStatesByCompanyIdAsync(Guid companyId,
        CancellationToken cancellationToken = default);

    Task CreateAsync(CreateWarehouseDto createWarehouseDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateWarehouseDto updateWarehouseDto, CancellationToken cancellationToken = default);

    Task<WarehouseStateResponseDto> GetStateByResponsiblePersonIdAsync(Guid responsiblePersonId,
        CancellationToken cancellationToken=default);
}