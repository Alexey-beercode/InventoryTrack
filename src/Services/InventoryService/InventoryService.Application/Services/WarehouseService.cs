using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public WarehouseService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task<WarehouseResponseDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);

        if (warehouse is null)
        {
            throw new EntityNotFoundException("Warehouse", id);
        }

        return _mapper.Map<WarehouseResponseDto>(warehouse);
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetByTypeAsync(WarehouseType warehouseType, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByTypeAsync(warehouseType, cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByCompanyIdAsync(companyId, cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetByResponsiblePersonIdAsync(Guid responsiblePersonId, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByResponsiblePersonIdAsync(responsiblePersonId, cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByNameAsync(name, cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetByLocationAsync(string location, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByLocationAsync(location, cancellationToken);
        return _mapper.Map<IEnumerable<WarehouseResponseDto>>(warehouses);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);

        if (warehouse is null)
        {
            throw new EntityNotFoundException("Warehouse", id);
        }

        _unitOfWork.Warehouses.Delete(warehouse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}