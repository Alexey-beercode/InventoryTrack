using InventoryService.Application.DTOs.Request.Supplier;
using InventoryService.Application.DTOs.Response.Supplier;
using InventoryService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly ILogger<SupplierController> _logger;

        public SupplierController(ISupplierService supplierService, 
            ILogger<SupplierController> logger)
        {
            _supplierService = supplierService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierResponseDto>>> GetAllSuppliers(
            CancellationToken cancellationToken)
        {
            var suppliers = await _supplierService.GetAllAsync(cancellationToken);
            return Ok(suppliers);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplierById(Guid id, 
            CancellationToken cancellationToken)
        {
            var supplier = await _supplierService.GetByIdAsync(id, cancellationToken);
            return Ok(supplier);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplierByName(string name, 
            CancellationToken cancellationToken)
        {
            var supplier = await _supplierService.GetByNameAsync(name, cancellationToken);
            return Ok(supplier);
        }

        [HttpGet("by-account/{accountNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SupplierResponseDto>> GetSupplierByAccountNumber(
            string accountNumber, CancellationToken cancellationToken)
        {
            var supplier = await _supplierService.GetByAccountNumber(accountNumber, cancellationToken);
            return Ok(supplier);
        }

        [HttpPost]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSupplier([FromBody] CreateSupplierDto createSupplierDto, 
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new supplier: {Name}", createSupplierDto.Name);
            await _supplierService.CreateAsync(createSupplierDto, cancellationToken);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSupplier(Guid id, 
            CancellationToken cancellationToken)
        {
            await _supplierService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}