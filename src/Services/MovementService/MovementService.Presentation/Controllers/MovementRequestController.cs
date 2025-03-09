using BookingService.Presentation.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovementService.Application.DTOs.Request.MovementRequest;
using MovementService.Application.DTOs.Response.MovementRequest;
using MovementService.Application.Interfaces.Services;
using MovementService.Domain.Enums;

namespace MovementService.Presentation.Controllers;

[ApiController]
[Route("api/movement-requests")]
public class MovementRequestController : ControllerBase
{
    private readonly IMovementRequestService _movementRequestService;
    private readonly LoggerHelper<MovementRequestController> _logger;

    public MovementRequestController(IMovementRequestService movementRequestService, ILogger<MovementRequestController> logger)
    {
        _movementRequestService = movementRequestService;
        _logger = new LoggerHelper<MovementRequestController>(logger);
    }

    // Создание новой заявки на перемещение (роль: Department Head)
    [HttpPost]
    [Authorize(Policy = "Department Head")]
    public async Task<ActionResult> CreateMovementRequest([FromBody] CreateMovementRequestDto dto, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Create Movement Request", "item", dto.ItemId.ToString());
        await _movementRequestService.CreateMovementRequestAsync(dto, cancellationToken);
        _logger.LogEndOfOperation("Create Movement Request", "created movement request");
        return Ok();
    }
    
    [HttpPost("{id}/approve")]
    [Authorize(Policy = "Warehouse Manager")]
    public async Task<ActionResult> ApproveMovementRequest([FromBody] ChangeStatusDto changeStatusDto, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Approve Movement Request", "requestId", changeStatusDto.RequestId.ToString());
        await _movementRequestService.CompleteMovementRequestAsync(changeStatusDto, cancellationToken);
        _logger.LogEndOfOperation("Approve Movement Request", "approved movement request");
        return Ok();
    }
    
    [HttpPost("{id}/reject")]
    [Authorize(Policy = "Warehouse Manager")]
    public async Task<ActionResult> RejectMovementRequest(ChangeStatusDto changeStatusDto, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Reject Movement Request", "requestId", changeStatusDto.RequestId.ToString());
        await _movementRequestService.RejectMovementRequestAsync(changeStatusDto, cancellationToken);
        _logger.LogEndOfOperation("Reject Movement Request", "rejected movement request");
        return Ok();
    }

    // Удаление заявки на перемещение (роль: Accountant)
    [HttpDelete("{id}")]
    [Authorize(Policy = "Accountant")]
    public async Task<ActionResult> DeleteMovementRequest(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Delete Movement Request", "requestId", id.ToString());
        await _movementRequestService.DeleteMovementRequestAsync(id, cancellationToken);
        _logger.LogEndOfOperation("Delete Movement Request", "deleted movement request");
        return Ok();
    }

    // Получение заявки на перемещение по ID (роль: все роли)
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<MovementRequestResponseDto>> GetMovementRequestById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Get Movement Request by ID", "requestId", id.ToString());
        var result = await _movementRequestService.GetMovementRequestByIdAsync(id, cancellationToken);
        _logger.LogEndOfOperation("Get Movement Request by ID", "retrieved movement request");
        return Ok(result);
    }

    // Получение всех заявок по статусу (роль: все роли)
    [HttpGet("status/{status}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MovementRequestResponseDto>>> GetMovementRequestsByStatus(string status, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Get Movement Requests by Status", "status", status);
        var movementStatus = Enum.Parse<MovementRequestStatus>(status, true);
        var result = await _movementRequestService.GetMovementRequestsByStatusAsync(movementStatus, cancellationToken);
        _logger.LogEndOfOperation("Get Movement Requests by Status", "retrieved movement requests by status");
        return Ok(result);
    }

    // Получение всех заявок по складу (роль: все роли)
    [HttpGet("warehouse/{warehouseId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<MovementRequestResponseDto>>> GetMovementRequestsByWarehouse(Guid warehouseId, CancellationToken cancellationToken)
    {
        _logger.LogStartRequest("Get Movement Requests by Warehouse", "warehouseId", warehouseId.ToString());
        var result = await _movementRequestService.GetMovementRequestsByAnyWarehouseIdAsync(warehouseId, cancellationToken);
        _logger.LogEndOfOperation("Get Movement Requests by Warehouse", "retrieved movement requests by warehouse");
        return Ok(result);
    }
}