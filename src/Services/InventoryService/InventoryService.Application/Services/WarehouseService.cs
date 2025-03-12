using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.Warehouse;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.DTOs.Response.WarehouseType;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInventoryItemFacade _inventoryItemFacade;
    private readonly ILogger<WarehouseService> _logger;

    public WarehouseService(IMapper mapper, IUnitOfWork unitOfWork, IInventoryItemFacade inventoryItemFacade, ILogger<WarehouseService> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _inventoryItemFacade = inventoryItemFacade;
        _logger = logger;
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

    public async Task<IEnumerable<WarehouseStateResponseDto>> GetAllWarehousesStateAsync(
        CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetAllAsync(cancellationToken);
        var warehousesStates = new List<WarehouseStateResponseDto>();
        foreach (var warehouse in warehouses)
        {
            var state = await GetStateByIdAsync(warehouse.Id, cancellationToken);
            warehousesStates.Add(state);
        }

        return warehousesStates;
    }

    public async Task<WarehouseStateResponseDto> GetStateByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id, cancellationToken);
        var inventoryItems = await GetInventoryItemsByWarehouseAsync(id, cancellationToken);

        return new WarehouseStateResponseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Type = _mapper.Map<WarehouseTypeResponseDto>(warehouse.Type),
            ItemsCount = inventoryItems.Count(),
            Quantity = inventoryItems.Sum(iw => iw.WarehouseDetails
                .Where(wd => wd.WarehouseId == id) // Фильтруем только по текущему складу
                .Sum(wd => wd.Quantity)), // Складываем количество
            InventoryItems = inventoryItems,
            ResponsiblePersonId = warehouse.ResponsiblePersonId
        };
    }

    public async Task<WarehouseStateResponseDto> GetStateByResponsiblePersonIdAsync(Guid responsiblePersonId,
        CancellationToken cancellationToken)
    {
        var warehouses = (await GetByResponsiblePersonIdAsync(responsiblePersonId, cancellationToken)).ToList();
        var warehouse = warehouses.First();

        if (warehouse is null)
        {
            throw new EntityNotFoundException("Склад не найден");
        }
        var inventoryItems = await GetInventoryItemsByWarehouseAsync(warehouse.Id, cancellationToken);

        return new WarehouseStateResponseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Location = warehouse.Location,
            Type = _mapper.Map<WarehouseTypeResponseDto>(warehouse.Type),
            ItemsCount = inventoryItems.Count(),
            Quantity = inventoryItems.Sum(iw => iw.WarehouseDetails
                .Where(wd => wd.WarehouseId == warehouse.Id) // Фильтруем только по текущему складу
                .Sum(wd => wd.Quantity)), // Складываем количество
            InventoryItems = inventoryItems,
            ResponsiblePersonId = warehouse.ResponsiblePersonId
        };
    }

    public async Task<IEnumerable<WarehouseStateResponseDto>> GetWarehousesStatesByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var warehouses = await _unitOfWork.Warehouses.GetByCompanyIdAsync(companyId, cancellationToken);

        var states = new List<WarehouseStateResponseDto>();
        foreach (var warehouse in warehouses)
        {
            var state = await GetStateByIdAsync(warehouse.Id, cancellationToken);
            states.Add(state);
        }

        return states;
    }

    public async Task CreateAsync(CreateWarehouseDto createWarehouseDto, CancellationToken cancellationToken = default)
    {
        var warehouse = _mapper.Map<Warehouse>(createWarehouseDto);
        await CheckSamePersonAsync(createWarehouseDto.AccountantId,createWarehouseDto.ResponsiblePersonId,cancellationToken);
        await _unitOfWork.Warehouses.CreateAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateWarehouseDto updateWarehouseDto, CancellationToken cancellationToken = default)
    {
        var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(updateWarehouseDto.Id, cancellationToken);
        if (warehouse is null)
        {
            throw new EntityNotFoundException("Склад", updateWarehouseDto.Id);
        }
        
        await CheckSamePersonAsync(updateWarehouseDto.AccountantId, updateWarehouseDto.ResponsiblePersonId, cancellationToken,warehouse);
        _mapper.Map(updateWarehouseDto, warehouse);
        _unitOfWork.Warehouses.Update(warehouse);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<InventoryItemResponseDto>> GetInventoryItemsByWarehouseAsync(Guid warehouseId, 
        CancellationToken cancellationToken = default)
    {
        var inventoryItemsDtos = new List<InventoryItemResponseDto>();

        var itemsWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        if (itemsWarehouses == null)
        {
            _logger.LogWarning("Для склада {WarehouseId} не найдено привязанных товаров.", warehouseId);
            return inventoryItemsDtos; // Возвращаем пустой список
        }

        foreach (var itemWarehouse in itemsWarehouses)
        {
            var inventoryItem = await _unitOfWork.InventoryItems.GetByIdCreatedAsync(itemWarehouse.ItemId, cancellationToken);
            if (inventoryItem == null)
            {
                _logger.LogWarning("Инвентарный предмет с ID {ItemId} не найден.", itemWarehouse.ItemId);
                continue;
            }

            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(inventoryItem.Id, cancellationToken);
            if (itemWarehouses == null)
            {
                _logger.LogWarning("Для предмета {ItemId} не найдено привязок к складам.", inventoryItem.Id);
                itemWarehouses = new List<InventoriesItemsWarehouses>(); // Даем пустой список
            }

            try
            {
                var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, inventoryItem, cancellationToken);
                inventoryItemsDtos.Add(itemDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении DTO для инвентарного предмета {ItemId}.", inventoryItem.Id);
            }
        }

        return inventoryItemsDtos;
    }

    private async Task CheckSamePersonAsync(Guid accountantId, Guid responsiblePersonId, 
        CancellationToken cancellationToken = default, Warehouse? warehouse = null)
    {

        if (accountantId == responsiblePersonId)
        {
            return;
        }
        var warehousesWithSamePerson = await _unitOfWork.Warehouses
            .GetByResponsiblePersonIdAsync(responsiblePersonId, cancellationToken);
    
        if (warehousesWithSamePerson == null || !warehousesWithSamePerson.Any())
        {
            _logger.LogInformation("Нет складов с ответственным лицом {ResponsiblePersonId}.", responsiblePersonId);
            return;
        }

        foreach (var warehouseWithSamePerson in warehousesWithSamePerson)
        {
            if (warehouse == null || warehouseWithSamePerson.Id != warehouse.Id)
            {
                _logger.LogInformation(
                    "Обновление склада {WarehouseId}: смена ответственного с {OldPersonId} на {NewPersonId}.",
                    warehouseWithSamePerson.Id, responsiblePersonId, accountantId);

                warehouseWithSamePerson.ResponsiblePersonId = accountantId;
                _unitOfWork.Warehouses.Update(warehouseWithSamePerson);
            }
        }
    }

}