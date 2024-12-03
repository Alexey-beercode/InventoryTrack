using MediatR;
using ReportService.Application.Exceptions;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.Delete;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand>
{
    private readonly IReportRepository _reportRepository;

    public DeleteCommandHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var report = await _reportRepository.GetByIdAsync(request.Id);
        if (report is null)
        {
            throw new EntityNotFoundException("Report", request.Id);
        }

        await _reportRepository.DeleteAsync(request.Id);
    }
}