using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Application.Interfaces.Services;
using WriteOffService.Domain.Enums;

namespace WriteOffService.Presentation.Controllers
{
    [ApiController]
    [Route("api/write-off-requests")]
    [Authorize]
    public class WriteOffRequestController : ControllerBase
    {
        private readonly IWriteOffRequestService _writeOffRequestService;
        private readonly ILogger<WriteOffRequestController> _logger;

        public WriteOffRequestController(IWriteOffRequestService writeOffRequestService,
            ILogger<WriteOffRequestController> logger)
        {
            _writeOffRequestService = writeOffRequestService;
            _logger = logger;
        }

        [HttpGet("warehouse/{warehouseId:guid}")]
        [Authorize(Policy = "Warehouse Manager")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WriteOffRequestResponseDto>>> GetByWarehouseId(
            Guid warehouseId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting write-off requests for warehouse: {WarehouseId}", warehouseId);
            var requests = await _writeOffRequestService.GetByWarehouseIdAsync(warehouseId, cancellationToken);
            return Ok(requests);
        }

        [HttpGet]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WriteOffRequestResponseDto>>> GetFiltered(
            [FromQuery] WriteOffRequestFilterDto filterDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting filtered write-off requests");
            var requests = await _writeOffRequestService.GetFilteredPagedRequestsAsync(filterDto, cancellationToken);
            return Ok(requests);
        }

        [HttpGet("status/{status}")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<WriteOffRequestResponseDto>>> GetByStatus(
            RequestStatus status,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting write-off requests by status: {Status}", status);
            var requests = await _writeOffRequestService.GetByStatusAsync(status, cancellationToken);
            return Ok(requests);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<WriteOffRequestResponseDto>> GetById(
            Guid id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting write-off request by id: {Id}", id);
            var request = await _writeOffRequestService.GetByIdAsync(id, cancellationToken);
            if (request == null)
                return NotFound();
            return Ok(request);
        }

        [HttpPost]
        [Authorize(Policy = "WarehouseOrDepartmentHead")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create(
            [FromBody] CreateWriteOffRequestDto createDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating new write-off request");
            await _writeOffRequestService.CreateAsync(createDto, cancellationToken);
            return Created();
        }

        [HttpPut]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(
            [FromBody] UpdateWriteOffRequestDto updateDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating write-off request: {Id}", updateDto.Id);
            await _writeOffRequestService.UpdateAsync(updateDto, cancellationToken);
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Policy = "Warehouse Manager,Department Head")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting write-off request: {Id}", id);
            await _writeOffRequestService.DeleteWriteOffRequestAsync(id, cancellationToken);
            return Ok();
        }
        [HttpPut("{id:guid}/approve")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Approve(
            Guid id,
            [FromBody] ApproveWriteOffRequestDto approveDto,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Approving write-off request: {Id}", id);
            approveDto.Id = id;
            await _writeOffRequestService.ApproveAsync(approveDto, cancellationToken);
            return Ok();
        }

        [HttpPost("{id:guid}/upload-documents")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadDocuments(
            Guid id,
            [FromForm] List<IFormFile> documents,
            CancellationToken cancellationToken)
        {
            return Ok();
        }


        [HttpPut("reject/{id:guid}")]
        [Authorize(Policy = "Accountant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Reject(
            Guid id,
            [FromQuery] Guid approvedByUserId,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Rejecting write-off request: {Id}", id);
            await _writeOffRequestService.RejectAsync(id, approvedByUserId, cancellationToken);
            return Ok();
        }
    }
}