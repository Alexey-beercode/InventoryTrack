using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MovementService.Domain.Interfaces.UnitOfWork;

namespace MovementService.Presentation.Controllers;

[ApiController]
[Route("api/movement/messaging")]
public class MessagingController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagingController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("report-data")]
    public async Task<ActionResult<BsonDocument>> GetMovementReportData(
        [FromQuery] int dateSelect,
        CancellationToken cancellationToken)
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

        var movements = await _unitOfWork.MovementRequests.GetByDateRangeAsync(startDate, endDate, cancellationToken);

        var response = new BsonDocument
        {
            {
                "Movements", new BsonArray(movements.Select(movement => new BsonDocument
                {
                    { "ItemId", movement.ItemId.ToString() },
                    { "SourceWarehouseId", movement.SourceWarehouseId.ToString() },
                    { "DestinationWarehouseId", movement.DestinationWarehouseId.ToString() },
                    { "Quantity", movement.Quantity },
                    { "Status", movement.Status.ToString() },
                    { "RequestDate", movement.RequestDate },
                    { "ApprovedByUserId", movement.ApprovedByUserId.ToString() }
                }))
            }
        };

        return Content(response.ToJson(), "application/json");
    }
}