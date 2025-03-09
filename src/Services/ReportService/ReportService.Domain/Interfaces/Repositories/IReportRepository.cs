using System.Linq.Expressions;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;

namespace ReportService.Domain.Interfaces.Repositories;

public interface IReportRepository : IBaseRepository<Report>
{
    Task<IEnumerable<Report>> GetByTypeAsync(ReportType reportType,CancellationToken cancellationToken=default);
    Task<IEnumerable<Report>> GetByFilterAsync(Expression<Func<Report, bool>> filter,CancellationToken cancellationToken=default);
    Task<IEnumerable<Report>> GetPaginatedAsync(int pageNumber, int pageSize,CancellationToken cancellationToken=default);
    Task<IEnumerable<Report>> GetByDateRangeAsync(DateTime startDate, DateTime endDate,CancellationToken cancellationToken=default);
    Task<IEnumerable<Report>> GetByDateSelectAsync(DateSelect dateSelect, CancellationToken cancellationToken = default);
Task<IEnumerable<Report>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
}