using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.Interfaces.Facades;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Domain.Interfaces.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace InventoryService.Application.Services;

public class InventoryItemService : IInventoryItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IDocumentService _documentService;
    private readonly IInventoryItemFacade _inventoryItemFacade;
    private readonly ILogger<InventoryItemService> _logger;

    public InventoryItemService(IUnitOfWork unitOfWork, IMapper mapper, IDocumentService documentService, IInventoryItemFacade inventoryItemFacade, ILogger<InventoryItemService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _documentService = documentService;
        _inventoryItemFacade = inventoryItemFacade;
        _logger = logger;
    }

    public async Task CreateInventoryItemAsync(CreateInventoryItemDto dto, CancellationToken cancellationToken = default)
    {
        var existingBatch = await _unitOfWork.InventoryItems.GetByNameAndBatchAsync(dto.Name, dto.BatchNumber, cancellationToken);
        if (existingBatch is not null)
        {
            throw new AlreadyExistsException($"Item with name '{dto.Name}' and batch '{dto.BatchNumber}' already exists");
        }

        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(dto.SupplierId, cancellationToken)
                       ?? throw new EntityNotFoundException($"Supplier with ID '{dto.SupplierId}' not found.");
        var warehouse=await _unitOfWork.Warehouses.GetByIdAsync(dto.WarehouseId, cancellationToken) 
                      ?? throw new EntityNotFoundException($"Warehouse with ID '{dto.WarehouseId}' not found.");
        var inventoryItem = _mapper.Map<InventoryItem>(dto);
        inventoryItem.DocumentId = dto.DocumentId;
        inventoryItem.SupplierId = supplier.Id;
        inventoryItem.Status = InventoryItemStatus.Created;
        inventoryItem.DeliveryDate=inventoryItem.DeliveryDate.ToUniversalTime();
        inventoryItem.ExpirationDate = inventoryItem.ExpirationDate.ToUniversalTime();
        await _unitOfWork.InventoryItems.CreateAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var inventoryWarehouse = new InventoriesItemsWarehouses
        {
            ItemId = inventoryItem.Id,
            WarehouseId = warehouse.Id,
            Quantity = dto.Quantity
        };

        await _unitOfWork.InventoriesItemsWarehouses.CreateAsync(inventoryWarehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AddDocumentToInventoryItemAsync(DocumentDto file,string inventoryItemName, CancellationToken cancellationToken = default)
    {
        var existingItem = await _unitOfWork.InventoryItems.GetByNameAsync(inventoryItemName);
        if (existingItem is null)
        {
            throw new EntityNotFoundException("Item does not exists");
        }
        var document = await _documentService.CreateDocumentAsync(file, cancellationToken)
                       ?? throw new InvalidOperationException("Document is invalid or incomplete.");
        existingItem.DocumentId=document.Id;
        _unitOfWork.InventoryItems.Update(existingItem);
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
    
    public async Task<IEnumerable<InventoryItemResponseDto>> GetByNameAllBatchesAsync(string name, CancellationToken cancellationToken = default)
    {
        // В отличие от GetByNameAsync, этот метод возвращает ВСЕ партии товара
        var items = await _unitOfWork.InventoryItems.GetAllByNameAsync(name, cancellationToken);
        var itemsDtos = new List<InventoryItemResponseDto>();
    
        foreach (var item in items)
        {
            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
            var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
            itemsDtos.Add(itemDto);
        }
    
        return itemsDtos;
    }

// ✅ ИСПРАВЛЕННЫЙ метод в InventoryItemService
public async Task<IEnumerable<BatchInfoDto>> GetBatchesByItemNameAsync(string itemName, Guid? warehouseId = null, CancellationToken cancellationToken = default)
{
    _logger.LogInformation("🔍 Поиск партий для товара: {ItemName}, склад: {WarehouseId}", itemName, warehouseId);

    // Получаем все товары с данным именем
    var items = await _unitOfWork.InventoryItems.GetAllByNameAsync(itemName, cancellationToken);
    
    if (!items.Any())
    {
        _logger.LogWarning("⚠ Товары с именем '{ItemName}' не найдены", itemName);
        return new List<BatchInfoDto>();
    }

    var batches = new List<BatchInfoDto>();
    var batchGroups = items.GroupBy(i => i.BatchNumber);

    foreach (var batchGroup in batchGroups)
    {
        long totalQuantity = 0;
        bool hasItemsInWarehouse = false;

        // Для каждого item в партии получаем количество со складов
        foreach (var item in batchGroup)
        {
            var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
            
            // ✅ ИСПРАВЛЕНИЕ: Фильтруем по складу, если указан
            if (warehouseId.HasValue)
            {
                itemWarehouses = itemWarehouses.Where(iw => iw.WarehouseId == warehouseId.Value);
            }

            var warehouseQuantity = itemWarehouses.Sum(iw => iw.Quantity);
            totalQuantity += warehouseQuantity;

            // Проверяем, есть ли товары этой партии на нужном складе
            if (warehouseQuantity > 0)
            {
                hasItemsInWarehouse = true;
            }
        }

        // ✅ ИСПРАВЛЕНИЕ: Добавляем партию только если есть товары на нужном складе
        if (warehouseId.HasValue && !hasItemsInWarehouse)
        {
            _logger.LogDebug("🔍 Партия {BatchNumber} пропущена - нет товаров на складе {WarehouseId}", 
                batchGroup.Key, warehouseId);
            continue;
        }

        // ✅ Также проверяем, что общее количество больше 0
        if (totalQuantity <= 0)
        {
            _logger.LogDebug("🔍 Партия {BatchNumber} пропущена - количество = 0", batchGroup.Key);
            continue;
        }

        var batch = new BatchInfoDto
        {
            BatchNumber = batchGroup.Key,
            ItemsCount = batchGroup.Count(),
            ManufactureDate = batchGroup.Min(i => i.DeliveryDate),
            ExpirationDate = batchGroup.Min(i => i.ExpirationDate),
            ManufacturerName = batchGroup.First().Supplier?.Name ?? "",
            TotalQuantity = totalQuantity
        };

        _logger.LogInformation("✅ Найдена партия: {BatchNumber}, количество: {TotalQuantity}", 
            batch.BatchNumber, batch.TotalQuantity);

        batches.Add(batch);
    }

    _logger.LogInformation("🔍 Итого найдено партий для товара '{ItemName}': {BatchCount}", itemName, batches.Count);
    return batches;
}
    
// Обновленный метод GetInventoryItemsByBatchNumberAsync в InventoryItemService

public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByBatchNumberAsync(string batchNumber, CancellationToken cancellationToken = default)
{
    Console.WriteLine($"🔍 InventoryItemService: Поиск партии '{batchNumber}'");
    
    var items = await _unitOfWork.InventoryItems.GetByBatchNumberAsync(batchNumber, cancellationToken);
    
    Console.WriteLine($"🔍 InventoryItemService: Найдено {items.Count()} товаров");
    
    var itemsDtos = new List<InventoryItemResponseDto>();

    foreach (var item in items)
    {
        Console.WriteLine($"🔍 Обработка товара: {item.Name}, ID: {item.Id}, BatchNumber: '{item.BatchNumber}', Status: {item.Status}");
        
        var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
        
        Console.WriteLine($"🔍 Найдено складских записей: {itemWarehouses.Count()}");
        
        var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
        itemsDtos.Add(itemDto);
    }

    Console.WriteLine($"🔍 InventoryItemService: Возвращаем {itemsDtos.Count} DTO");
    return itemsDtos;
}

// Добавляем метод для поиска партий без фильтра по статусу
public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByBatchNumberAllStatusesAsync(string batchNumber, CancellationToken cancellationToken = default)
{
    // Используем альтернативный метод из репозитория
    var items = await _unitOfWork.InventoryItems.GetByBatchNumberAllStatusesAsync(batchNumber, cancellationToken);
    var itemsDtos = new List<InventoryItemResponseDto>();

    foreach (var item in items)
    {
        var itemWarehouses = await _unitOfWork.InventoriesItemsWarehouses.GetByItemIdAsync(item.Id, cancellationToken);
        var itemDto = await _inventoryItemFacade.GetFullInventoryItemDto(itemWarehouses, item, cancellationToken);
        itemsDtos.Add(itemDto);
    }

    return itemsDtos;
}
    
    public async Task WriteOffBatchAsync(string batchNumber, CancellationToken cancellationToken = default)
    {
        var batchItems = await GetInventoryItemsByBatchNumberAsync(batchNumber, cancellationToken);

        if (!batchItems.Any())
        {
            throw new EntityNotFoundException($"Batch '{batchNumber}' not found.");
        }

        // Списываем все товары из партии
        foreach (var item in batchItems)
        {
            foreach (var warehouseDetail in item.WarehouseDetails)
            {
                await WriteOffItemAsync(item.Id, warehouseDetail.WarehouseId, warehouseDetail.Quantity, cancellationToken);
            }
        }
    }

}