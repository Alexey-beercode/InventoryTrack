using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WriteOffService.Application.DTOs.Request.WriteOffRequest;
using WriteOffService.Application.DTOs.Response.WriteOffRequest;
using WriteOffService.Application.Interfaces.Services;

namespace WriteOffService.Presentation.Controllers;

[ApiController]
[Route("api/writeoff/messaging")]
public class MessagingController : ControllerBase
{
    private readonly IWriteOffRequestService _writeOffRequestService;
    private readonly ILogger<MessagingController> _logger;

    public MessagingController(IWriteOffRequestService writeOffRequestService, ILogger<MessagingController> logger)
    {
        _writeOffRequestService = writeOffRequestService;
        _logger = logger;
    }

    [HttpGet("report-data")]
    public async Task<ActionResult<BsonDocument>> GetWriteOffReportData(
        [FromQuery] Guid companyId,
        [FromQuery] int dateSelect,
        CancellationToken cancellationToken)
    {
        if (companyId == Guid.Empty)
        {
            _logger.LogError("Invalid CompanyId provided");
            return BadRequest("Invalid CompanyId");
        }

        if (dateSelect < 0 || dateSelect > 3)
        {
            _logger.LogError("Invalid dateSelect provided: {DateSelect}", dateSelect);
            return BadRequest("Invalid dateSelect");
        }

        _logger.LogInformation("Processing report-data for CompanyId: {CompanyId}, DateSelect: {DateSelect}", companyId, dateSelect);

        var writeOffs = await GetWriteOffsAsync(companyId, dateSelect, cancellationToken);
        if (!writeOffs.Any())
        {
            _logger.LogWarning("No write-offs found for CompanyId: {CompanyId}, DateSelect: {DateSelect}", companyId, dateSelect);
            return NotFound("No data found for the provided parameters");
        }

        var response = GetData(writeOffs);
        
        _logger.LogInformation("Returning data: {Response}", response.ToJson());
        return Content(response.ToJson(), "application/json");
    }

    [HttpGet("report-data/by-warehouse")]
    public async Task<ActionResult<BsonDocument>> GetWriteOffReportDataByWarehouse(
        [FromQuery] Guid warehouseId,
        [FromQuery] int dateSelect,
        CancellationToken cancellationToken)
    {
        if (warehouseId == Guid.Empty)
        {
            _logger.LogError("Invalid WarehouseId provided");
            return BadRequest("Invalid WarehouseId");
        }

        if (dateSelect < 0 || dateSelect > 3)
        {
            _logger.LogError("Invalid dateSelect provided: {DateSelect}", dateSelect);
            return BadRequest("Invalid dateSelect");
        }

        _logger.LogInformation("Processing report-data for WarehouseId: {WarehouseId}, DateSelect: {DateSelect}", warehouseId, dateSelect);

        var writeOffs = await GetWriteOffsAsync(null, dateSelect, cancellationToken);

        var filteredWriteOffs = writeOffs.Where(w => w.WarehouseId == warehouseId);
        
        if (!filteredWriteOffs.Any())
        {
            _logger.LogWarning("No write-offs found for WarehouseId: {WarehouseId}, DateSelect: {DateSelect}", warehouseId, dateSelect);
            return NotFound("No data found for the provided parameters");
        }

        var response = GetData(filteredWriteOffs);
        
        _logger.LogInformation("Returning data: {Response}", response.ToJson());
        return Content(response.ToJson(), "application/json");
    }

    private async Task<IEnumerable<WriteOffRequestResponseDto>> GetWriteOffsAsync(Guid? companyId, int dateSelect, CancellationToken cancellationToken)
    {
        var startDate = dateSelect switch
        {
            0 => DateTime.UtcNow.AddDays(-1),
            1 => DateTime.UtcNow.AddDays(-7),
            2 => DateTime.UtcNow.AddMonths(-1),
            3 => DateTime.UtcNow.AddYears(-1),
            _ => throw new ArgumentOutOfRangeException(nameof(dateSelect), "Invalid date selection")
        };

        var endDate = DateTime.UtcNow;
        var filter = new WriteOffRequestFilterDto
        {
            CompanyId = (Guid)companyId!,
            IsPaginated = false,
            StartDate = startDate,
            EndDate = endDate
        };
        var all = await _writeOffRequestService.GetAllAsync(cancellationToken);
        _logger.LogInformation(JsonSerializer.Serialize(all));
       return await _writeOffRequestService.GetFilteredPagedRequestsAsync(filter, cancellationToken);
    }

    private BsonDocument GetData(IEnumerable<WriteOffRequestResponseDto> writeOffs)
    {
        return new BsonDocument
        {
            {
                "WriteOffs", new BsonArray(writeOffs.Select(writeOff => new BsonDocument
                {
                    { "ItemId", writeOff.ItemId.ToString() },
                    { "WarehouseId", writeOff.WarehouseId.ToString() },
                    { "Quantity", writeOff.Quantity },
                    { "Status", writeOff.Status.Name },
                    { "RequestDate", writeOff.RequestDate },
                    { "Reason", writeOff.Reason.Reason },
                    { "ApprovedByUser", writeOff.ApprovedByUserId.ToString() },
                    {"CompanyId", writeOff.CompanyId.ToString() },
                }))
            }
        };
    }
}
