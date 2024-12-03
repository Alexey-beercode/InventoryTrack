using MediatR;

namespace ReportService.Application.UseCases.Report.Delete;

public class DeleteCommand : IRequest
{
    public Guid Id { get; set; }
}