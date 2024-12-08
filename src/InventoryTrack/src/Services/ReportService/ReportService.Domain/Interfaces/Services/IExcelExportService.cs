using ReportService.Domain.Entities;

namespace ReportService.Domain.Interfaces.Services
{
    public interface IExcelExportService
    {
        Task<byte[]> GenerateExcelReportAsync(Report report);
    }
}