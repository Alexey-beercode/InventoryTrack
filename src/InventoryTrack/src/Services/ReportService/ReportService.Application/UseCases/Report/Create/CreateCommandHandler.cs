/*using AutoMapper;
using MassTransit;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ReportService.Domain.Interfaces.Repositories;

namespace ReportService.Application.UseCases.Report.Create
{
    public class CreateCommandHandler : IRequestHandler<CreateCommand>
    {
        private readonly IRequestClient<CreateReportMessage> _requestClient;
        private readonly IReportRepository _reportRepository;

        public CreateCommandHandler(IRequestClient<CreateReportMessage> requestClient, IReportRepository reportRepository)
        {
            _requestClient = requestClient;
            _reportRepository = reportRepository;
        }

        public async Task Handle(CreateCommand request, CancellationToken cancellationToken)
        {
            // Формируем запрос
            var response = await _requestClient.GetResponse<CreateReportResponse>(
                new CreateReportMessage
                {
                    ReportType = request.ReportType.ToString()
                });

            var reportData = response.Message.Data;
            var report = new Domain.Entities.Report()
            {
                CreatedAt = DateTime.UtcNow, 
                Data = reportData, 
                DateSelect = request.DateSelect,
                ReportType = request.ReportType, 
                Name = DateTime.UtcNow.ToString()
            };

            await _reportRepository.CreateAsync(report);
        }
    }
}*/