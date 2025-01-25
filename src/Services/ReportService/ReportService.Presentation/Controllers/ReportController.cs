using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReportService.Application.UseCases.Report.Create;
using ReportService.Application.UseCases.Report.Delete;
using ReportService.Application.UseCases.Report.GetAll;
using ReportService.Application.UseCases.Report.GetByDateRange;
using ReportService.Application.UseCases.Report.GetByDateSelect;
using ReportService.Application.UseCases.Report.GetById;
using ReportService.Application.UseCases.Report.GetByType;
using ReportService.Application.UseCases.Report.GetPaginated;
using ReportService.Domain.Enums;

namespace ReportService.Presentation.Controllers;

[ApiController]
[Route("api/report")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var report = await _mediator.Send(new GetReportByIdQuery { Id = id }, cancellationToken);
        
        return File(report.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{report.Name}.xlsx");
    }

    [HttpGet("by-date-range/{startDate}/{endDate}")]
    public async Task<IActionResult> GetByDateRangeAsync(DateTime startDate,DateTime endDate,
        CancellationToken cancellationToken = default)
    {
        var reports = await _mediator.Send(new GetByDateRangeQuery() { EndDate = endDate, StartDate = startDate },
            cancellationToken);
        return Ok(reports);
    }

    [HttpGet("by-date-select/{dateSelect}")]
    public async Task<IActionResult> GetByDateSelectAsync(DateSelect dateSelect,
        CancellationToken cancellationToken = default)
    {
        var reports = await _mediator.Send(new GetByDateSelectQuery() { DateSelect = dateSelect }, cancellationToken);
        return Ok(reports);
    }

    [HttpGet("by-type/{type}")]
    public async Task<IActionResult> GetByTypeAsync(ReportType type, CancellationToken cancellationToken = default)
    {
        var reports = await _mediator.Send(new GetReportsByTypeQuery() { ReportType = type }, cancellationToken);
        return Ok(reports);
    }

    [HttpGet("paginated")]
    public async Task<IActionResult> GetPaginatedAsync(
        [FromQuery] GetPaginatedReportsQuery query,
        CancellationToken cancellationToken = default)
    {
        var reports = await _mediator.Send(query, cancellationToken);
        return Ok(reports);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var reports = await _mediator.Send(new GetAllQuery(),cancellationToken);
        return Ok(reports);
    }

    [HttpDelete("/{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id,CancellationToken cancellationToken = default)
    {
        await _mediator.Send(new DeleteCommand() { Id = id }, cancellationToken);
        return Ok();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCommand createCommand,
        CancellationToken cancellationToken = default)
    {
        await _mediator.Send(createCommand, cancellationToken);
        return Created();
    }
}