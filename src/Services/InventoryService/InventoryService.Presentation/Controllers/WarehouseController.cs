using InventoryService.Application.DTOs.Request.Warehouse;
using InventoryService.Application.DTOs.Response.Warehouse;
using InventoryService.Application.Interfaces.Services;
using InventoryService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/warehouses")]
    [Authorize]
    public class WarehouseController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehouseController> _logger;

        public WarehouseController(IWarehouseService warehouseService, 
            ILogger<WarehouseController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetAllWarehouses(
            CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetAllAsync(cancellationToken);
            return Ok(warehouses);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseResponseDto>> GetWarehouseById(Guid id, 
            CancellationToken cancellationToken)
        {
            var warehouse = await _warehouseService.GetByIdAsync(id, cancellationToken);
            return Ok(warehouse);
        }

        [HttpGet("by-type/{warehouseType}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehousesByType(
            WarehouseType warehouseType, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetByTypeAsync(warehouseType, cancellationToken);
            return Ok(warehouses);
        }

        [HttpGet("by-company/{companyId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehousesByCompany(
            Guid companyId, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetByCompanyIdAsync(companyId, cancellationToken);
            return Ok(warehouses);
        }

        [HttpGet("by-responsible-person/{responsiblePersonId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehousesByResponsiblePerson(
            Guid responsiblePersonId, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetByResponsiblePersonIdAsync(responsiblePersonId, 
                cancellationToken);
            return Ok(warehouses);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehousesByName(
            string name, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetByNameAsync(name, cancellationToken);
            return Ok(warehouses);
        }

        [HttpGet("by-location/{location}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseResponseDto>>> GetWarehousesByLocation(
            string location, CancellationToken cancellationToken)
        {
            var warehouses = await _warehouseService.GetByLocationAsync(location, cancellationToken);
            return Ok(warehouses);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "WarehouseManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteWarehouse(Guid id, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting warehouse with ID: {Id}", id);
            await _warehouseService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateWarehouse([FromBody] CreateWarehouseDto createWarehouseDto,
            CancellationToken cancellationToken = default)
        {
            await _warehouseService.CreateAsync(createWarehouseDto, cancellationToken);
            return Created();
        }
        
        [HttpGet("states")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseStateResponseDto>>> GetAllWarehousesStates(
            CancellationToken cancellationToken)
        {
            var warehouseStates = await _warehouseService.GetAllWarehousesStateAsync(cancellationToken);
            return Ok(warehouseStates);
        }

        [HttpGet("states/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WarehouseStateResponseDto>> GetWarehouseStateById(Guid id, 
            CancellationToken cancellationToken)
        {
            var warehouseState = await _warehouseService.GetStateByIdAsync(id, cancellationToken);
            if (warehouseState == null)
            {
                return NotFound();
            }
            return Ok(warehouseState);
        }

        [HttpGet("states/by-company/{companyId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseStateResponseDto>>> GetWarehousesStatesByCompany(
            Guid companyId, CancellationToken cancellationToken)
        {
            var warehouseStates = await _warehouseService.GetWarehousesStatesByCompanyIdAsync(companyId, cancellationToken);
            return Ok(warehouseStates);
        }
        
        [HttpGet("states/by-person/{responsiblePersonId:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<WarehouseStateResponseDto>>> GetWarehousesStatesByResponsiblePersonIdAsync(
            Guid responsiblePersonId, CancellationToken cancellationToken)
        {
            var warehouseState = await _warehouseService.GetStateByResponsiblePersonIdAsync(responsiblePersonId, cancellationToken);
            return Ok(warehouseState);
        }

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateWarehouseDto dto,
            CancellationToken cancellationToken = default)
        {
            await _warehouseService.UpdateAsync(dto, cancellationToken);
            return Ok();
        }

    }
}