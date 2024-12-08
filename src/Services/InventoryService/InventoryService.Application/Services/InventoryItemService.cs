using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;

namespace InventoryService.Application.Services;

public class InventoryItemService : IInventoryItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentService _documentService;
    private readonly IInventoryItemFacade _inventoryItemFacade;

    public InventoryItemService(IUnitOfWork unitOfWork, IMapper mapper, IDocumentService documentService, IInventoryItemFacade inventoryItemFacade)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _documentService = documentService;
        _inventoryItemFacade = inventoryItemFacade;
    }

    public async Task CreateInventoryItemAsync(CreateInventoryItemDto dto, CancellationToken cancellationToken = default)
    {
        var existingItem = await _unitOfWork.InventoryItems.GetByNameAsync(dto.Name);
        if (existingItem is not null)
        {
            throw new AlreadyExistsException("Item already exists");
        }

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId, cancellationToken)
                       ?? throw new EntityNotFoundException($"Supplier with ID '{dto.SupplierId}' not found.");

        var document = await _documentService.CreateDocumentAsync(dto.DocumentFile, cancellationToken)
                       ?? throw new InvalidOperationException("Document is invalid or incomplete.");

        var inventoryItem = _mapper.Map<InventoryItem>(dto);
        inventoryItem.DocumentId = document.Id;
        inventoryItem.SupplierId = supplier.Id;
        inventoryItem.Status = InventoryItemStatus.Requested;

        await _unitOfWork.InventoryItems.CreateAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var inventoryWarehouse = new InventoriesItemsWarehouses
        {
            ItemId = inventoryItem.Id,
            WarehouseId = dto.WarehouseId,
            Quantity = dto.Quantity
        };

        await _unitOfWork.InventoriesItemsWarehouses.CreateAsync(inventoryWarehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryItemResponseDto> GetInventoryItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var inventoryItem = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken)
                            ?? throw new EntityNotFoundException($"InventoryItem with ID '{id}' not found.");

        var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(id, cancellationToken);
        return await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, inventoryItem, cancellationToken);
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetAllAsync(cancellationToken);
        var itemsDtos = new List<InventoryItemResponseDto>();

        foreach (var item in items)
        {
            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
            var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
            itemsDtos.Add(itemDto);
        }

        return itemsDtos;
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetFilteredItemsAsync(FilterInventoryItemDto filterInventoryItemDto, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetFilteredItemsAsync(
            filterInventoryItemDto.Name,
            filterInventoryItemDto.SupplierId,
            filterInventoryItemDto.ExpirationDateFrom,
            filterInventoryItemDto.ExpirationDateTo,
            filterInventoryItemDto.EstimatedValue);

        var itemsDtos = new List<InventoryItemResponseDto>();

        foreach (var item in items)
        {
            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
            var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
            itemsDtos.Add(itemDto);
        }

        return itemsDtos;
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        var itemsWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByWarehouseIdAsync(warehouseId, cancellationToken);

        var itemsDtos = new List<InventoryItemResponseDto>();
        foreach (var iw in itemsWarehouses.GroupBy(iw => iw.ItemId))
        {
            var item = iw.First().InventoryItem;
            var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(iw, item, cancellationToken);
            itemsDtos.Add(itemDto);
        }

        return itemsDtos;
    }

    public async Task MoveItemAsync(MoveItemDto moveItemDto, CancellationToken cancellationToken = default)
    {
        var sourceEntry = await _unitOfWork.InventoriesItemsWarehouses
            .GetByWarehouseIdAsync(moveItemDto.SourceWarehouseId, cancellationToken)
            .ContinueWith(task => task.Result.FirstOrDefault(iw => iw.ItemId == moveItemDto.ItemId), cancellationToken);

        if (sourceEntry == null || sourceEntry.Quantity < moveItemDto.Quantity)
        {
            throw new InvalidOperationException("Insufficient quantity in source warehouse.");
        }

        sourceEntry.Quantity -= moveItemDto.Quantity;

        if (sourceEntry.Quantity == 0)
        {
            _unitOfWork.InventoriesItemsWarehouses.Delete(sourceEntry);
        }
        else
        {
            _unitOfWork.InventoriesItemsWarehouses.Update(sourceEntry);
        }

        var destinationEntry = await _unitOfWork.InventoriesItemsWarehouses
            .GetByWarehouseIdAsync(moveItemDto.DestinationWarehouseId, cancellationToken)
            .ContinueWith(task => task.Result.FirstOrDefault(iw => iw.ItemId == moveItemDto.ItemId), cancellationToken);

        if (destinationEntry == null)
        {
            var newEntry = new InventoriesItemsWarehouses
            {
                ItemId = moveItemDto.ItemId,
                WarehouseId = moveItemDto.DestinationWarehouseId,
                Quantity = moveItemDto.Quantity
            };
            await _unitOfWork.InventoriesItemsWarehouses.CreateAsync(newEntry, cancellationToken);
        }
        else
        {
            destinationEntry.Quantity += moveItemDto.Quantity;
            _unitOfWork.InventoriesItemsWarehouses.Update(destinationEntry);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryItemResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByNameAsync(name, cancellationToken)
                   ?? throw new EntityNotFoundException($"InventoryItem with name '{name}' not found.");

        var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
        return await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetByStatusAsync(InventoryItemStatus status, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetByStatusAsync(status, cancellationToken);
        var itemsDtos = new List<InventoryItemResponseDto>();

        foreach (var item in items)
        {
            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
            var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
            itemsDtos.Add(itemDto);
        }

        return itemsDtos;
    }

    public async Task UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken)
                   ?? throw new EntityNotFoundException($"InventoryItem with ID '{id}' not found.");

        _mapper.Map(dto, item);
        _unitOfWork.InventoryItems.Update(item);

        var warehouseEntry = await _unitOfWork.InventoriesItemsWarehouses
            .GetByWarehouseIdAsync(dto.WarehouseId, cancellationToken)
            .ContinueWith(task => task.Result.FirstOrDefault(iw => iw.ItemId == id), cancellationToken);

        if (warehouseEntry != null)
        {
            warehouseEntry.Quantity = dto.Quantity;
            _unitOfWork.InventoriesItemsWarehouses.Update(warehouseEntry);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteInventoryItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken)
                   ?? throw new EntityNotFoundException($"InventoryItem with ID '{id}' not found.");

        var itemsWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(id, cancellationToken);
        foreach (var iw in itemsWarehouses)
        {
            _unitOfWork.InventoriesItemsWarehouses.Delete(iw);
        }

        await _documentService.DeleteDocumentAsync(item.DocumentId, cancellationToken);
        _unitOfWork.InventoryItems.Delete(item);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInventoryItemStatusAsync(ChangeInventoryItemStatusDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(dto.InventoryItemId, cancellationToken)
                   ?? throw new EntityNotFoundException($"InventoryItem with ID '{dto.InventoryItemId}' not found.");

        if (dto.Status == InventoryItemStatus.Rejected)
        {
            await DeleteInventoryItemAsync(dto.InventoryItemId, cancellationToken);
            return;
        }

        item.Status = dto.Status;
        _unitOfWork.InventoryItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task WriteOffItemAsync(Guid itemId, Guid warehouseId, long quantity, CancellationToken cancellationToken = default)
    {
        var entry = await _unitOfWork.InventoriesItemsWarehouses
            .GetByWarehouseIdAsync(warehouseId, cancellationToken)
            .ContinueWith(task => task.Result.FirstOrDefault(iw => iw.ItemId == itemId), cancellationToken);

        if (entry == null || entry.Quantity < quantity)
        {
            throw new InvalidOperationException("Insufficient quantity for write-off.");
        }

        entry.Quantity -= quantity;

        if (entry.Quantity == 0)
        {
            _unitOfWork.InventoriesItemsWarehouses.Delete(entry);
        }
        else
        {
            _unitOfWork.InventoriesItemsWarehouses.Update(entry);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

}