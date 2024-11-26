using InventoryService.Application.DTOs.Request.InventoryItem;
using InventoryService.Application.DTOs.Response.InventoryItem;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/inventory-items")]
    [Authorize]
    public class InventoryItemController : ControllerBase
    {
        private readonly IInventoryItemService _inventoryItemService;
        private readonly ILogger<InventoryItemController> _logger;

        public InventoryItemController(IInventoryItemService inventoryItemService, 
            ILogger<InventoryItemController> logger)
        {
            _inventoryItemService = inventoryItemService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Policy = "Warehouse Manager")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateInventoryItem([FromForm] CreateInventoryItemDto dto, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new inventory item: {Name}", dto.Name);
            await _inventoryItemService.CreateInventoryItemAsync(dto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemResponseDto>>> GetAllInventoryItems(
            CancellationToken cancellationToken)
        {
            var items = await _inventoryItemService.GetInventoryItemsAsync(cancellationToken);
            return Ok(items);
        }

        [HttpGet("filtered")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemResponseDto>>> GetFilteredItems(
            [FromQuery] FilterInventoryItemDto filterDto, CancellationToken cancellationToken)
        {
            var items = await _inventoryItemService.GetFilteredItemsAsync(filterDto, cancellationToken);
            return Ok(items);
        }

        [HttpGet("warehouse/{warehouseId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InventoryItemResponseDto>>> GetByWarehouse(
            Guid warehouseId, CancellationToken cancellationToken)
        {
            var items = await _inventoryItemService.GetInventoryItemsByWarehouseAsync(warehouseId, cancellationToken);
            return Ok(items);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemResponseDto>> GetById(Guid id, 
            CancellationToken cancellationToken)
        {
            var item = await _inventoryItemService.GetInventoryItemAsync(id, cancellationToken);
            return Ok(item);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemResponseDto>> GetByName(string name, 
            CancellationToken cancellationToken)
        {
            var item = await _inventoryItemService.GetByNameAsync(name, cancellationToken);
            return Ok(item);
        }

        [HttpGet("by-code/{uniqueCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InventoryItemResponseDto>> GetByUniqueCode(string uniqueCode, 
            CancellationToken cancellationToken)
        {
            var item = await _inventoryItemService.GetByUniqueCodeAsync(uniqueCode, cancellationToken);
            return Ok(item);
        }

        [HttpGet("by-status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InventoryItemResponseDto>>> GetByStatus(
            InventoryItemStatus status, CancellationToken cancellationToken)
        {
            var items = await _inventoryItemService.GetByStatusAsync(status, cancellationToken);
            return Ok(items);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Policy = "Warehouse Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateInventoryItem(Guid id, 
            [FromBody] UpdateInventoryItemDto dto, CancellationToken cancellationToken)
        {
            await _inventoryItemService.UpdateInventoryItemAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpPut("status")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus([FromBody] ChangeInventoryItemStatusDto dto, 
            CancellationToken cancellationToken)
        {
            await _inventoryItemService.UpdateInventoryItemStatusAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Warehouse Manager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteInventoryItem(Guid id, CancellationToken cancellationToken)
        {
            await _inventoryItemService.DeleteInventoryItemAsync(id, cancellationToken);
            return NoContent();
        }
    }
}