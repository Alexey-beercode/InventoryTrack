using AutoMapper;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Facades;

public class InventoryItemFacade : IInventoryItemFacade
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<InventoryItemFacade> _logger;

    public InventoryItemFacade(IUnitOfWork unitOfWork, IMapper mapper, ILogger<InventoryItemFacade> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<InventoryItemResponseDto> GetFullInventoryItemDto(
        IEnumerable<InventoriesItemsWarehouses> inventoriesItemsWarehouses,
        InventoryItem inventoryItem, 
        CancellationToken cancellationToken = default)
    {
        if (inventoryItem == null)
        {
            _logger.LogError("inventoryItem is null in GetFullInventoryItemDto");
            throw new ArgumentNullException(nameof(inventoryItem), "Inventory item cannot be null");
        }

        var inventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryItem);
        var warehousesInfoList = new List<WarehouseDetailsDto>();

        // Проверяем, есть ли документ у предмета
        Document? document = null;
        if (inventoryItem.DocumentId != null)
        {
            document = await _unitOfWork.Documents.GetByIdAsync(inventoryItem.DocumentId, cancellationToken);
            if (document == null)
            {
                _logger.LogWarning("Document with ID {DocumentId} not found", inventoryItem.DocumentId);
            }
        }

        foreach (var inventoryWarehouse in inventoriesItemsWarehouses)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(inventoryWarehouse.WarehouseId, cancellationToken);

            if (warehouse == null)
            {
                _logger.LogWarning("Warehouse with ID {WarehouseId} not found", inventoryWarehouse.WarehouseId);
                continue; // Пропускаем итерацию, если склад не найден
            }

            warehousesInfoList.Add(new WarehouseDetailsDto
            {
                Quantity = inventoryWarehouse.Quantity,
                WarehouseId = warehouse.Id,
                WarehouseName = warehouse.Name
            });
        }

        inventoryItemDto.WarehouseDetails = warehousesInfoList;
        inventoryItemDto.DocumentInfo = document != null ? _mapper.Map<DocumentInfoResponseDto>(document) : null;
        inventoryItemDto.Quantity = warehousesInfoList.Sum(a => a.Quantity);

        return inventoryItemDto;
    }

}