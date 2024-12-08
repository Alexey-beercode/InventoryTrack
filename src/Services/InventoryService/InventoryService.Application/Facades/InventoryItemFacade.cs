using AutoMapper;
using InventoryService.Application.DTOs.Response.Document;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Application.Facades;

public class InventoryItemFacade : IInventoryItemFacade
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public InventoryItemFacade(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<InventoryItemResponseDto> GetFullInventoryItemDto(IEnumerable<InventoriesItemsWarehouses> inventoriesItemsWarehouses,InventoryItem inventoryItem, CancellationToken cancellationToken = default)
    {
        var inventoryItemDto = _mapper.Map<InventoryItemResponseDto>(inventoryItem);
        var warehousesInfoList = new List<WarehouseDetailsDto>();
        var document = await _unitOfWork.Documents.GetByIdAsync(inventoryItem.DocumentId, cancellationToken);
        foreach (var inventoryWarehouse in inventoriesItemsWarehouses)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(inventoryWarehouse.WarehouseId,cancellationToken);
            warehousesInfoList.Add(new WarehouseDetailsDto(){Quantity = inventoryWarehouse.Quantity,WarehouseId = warehouse.Id,WarehouseName = warehouse.Name});
        }

        inventoryItemDto.WarehouseDetails = warehousesInfoList;
        inventoryItemDto.DocumentInfo = _mapper.Map<DocumentInfoResponseDto>(document);
        inventoryItemDto.Quantity = warehousesInfoList.Sum(a => a.Quantity);
        return inventoryItemDto;
    }
}