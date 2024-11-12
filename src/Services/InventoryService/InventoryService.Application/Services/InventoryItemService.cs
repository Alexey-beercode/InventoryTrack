using AutoMapper;
using BookingService.Application.Exceptions;
using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
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

    public InventoryItemService(IUnitOfWork unitOfWork, IMapper mapper, IDocumentService documentService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _documentService = documentService;
    }

    public async Task CreateInventoryItemAsync(CreateInventoryItemDto dto, CancellationToken cancellationToken = default)
    {
        var document = await _documentService.CreateDocumentAsync(dto.DocumentFile, cancellationToken);

        if (document is null)
        {
            throw new InvalidOperationException("Document is invalid");
        }
        
        var inventoryItem = _mapper.Map<InventoryItem>(dto);
        inventoryItem.DocumentId = document.Id;
        inventoryItem.Status = InventoryItemStatus.Requested; 

        await _unitOfWork.InventoryItems.CreateAsync(inventoryItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<InventoryItemResponseDto> GetInventoryItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken);
        
        if (item == null)
        {
            throw new EntityNotFoundException("InventoryItem", id); 
        }
        
        return _mapper.Map<InventoryItemResponseDto>(item);
    }
    
    public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<InventoryItemResponseDto>>(items);
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetFilteredItemsAsync(FilterInventoryItemDto filterInventoryItemDto, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetFilteredItemsAsync(filterInventoryItemDto.Name, filterInventoryItemDto.SupplierId, filterInventoryItemDto.WarehouseId, filterInventoryItemDto.ExpirationDateFrom, filterInventoryItemDto.ExpirationDateTo, filterInventoryItemDto.EstimatedValue);
        return _mapper.Map<IEnumerable<InventoryItemResponseDto>>(items);
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetInventoryItemsByWarehouseAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetByWarehouseIdAsync(warehouseId, cancellationToken);
        return _mapper.Map<IEnumerable<InventoryItemResponseDto>>(items);
    }

    public async Task<InventoryItemResponseDto> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByNameAsync(name, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"InventoryItem with name : {name}");
        }
        
        return _mapper.Map<InventoryItemResponseDto>(item);
    }

    public async Task<InventoryItemResponseDto> GetByUniqueCodeAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByUniqueCodeAsync(uniqueCode, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"InventoryItem with unique code : {uniqueCode}");
        }
            
        return _mapper.Map<InventoryItemResponseDto>(item);
    }

    public async Task<IEnumerable<InventoryItemResponseDto>> GetByStatusAsync(InventoryItemStatus status, CancellationToken cancellationToken = default)
    {
        var items = await _unitOfWork.InventoryItems.GetByStatusAsync(status, cancellationToken);
        return _mapper.Map<IEnumerable<InventoryItemResponseDto>>(items);
    }

    public async Task UpdateInventoryItemAsync(Guid id, UpdateInventoryItemDto dto, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException("InventoryItem", id);
        }
        
        _mapper.Map(dto, item);
        _unitOfWork.InventoryItems.Update(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    
    public async Task DeleteInventoryItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _unitOfWork.InventoryItems.GetByIdAsync(id, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException("InventoryItem", id);
        }
        
        await _documentService.DeleteDocumentAsync(item.DocumentId, cancellationToken);
        _unitOfWork.InventoryItems.Delete(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}