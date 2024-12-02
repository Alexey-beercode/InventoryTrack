using System.Linq.Expressions;
using MongoDB.Driver;
using ReportService.Domain.Entities;
using ReportService.Domain.Enums;
using ReportService.Domain.Interfaces.Repositories;
using ReportService.Infrastructure.Database;

namespace ReportService.Infrastructure.Repositories;

public class ReportRepository : BaseRepository<Report>, IReportRepository
{
    public ReportRepository(ReportDbContext context) : base(context, "reports")
    {
    }

    public async Task<IEnumerable<Report>> GetByTypeAsync(ReportType reportType,CancellationToken cancellationToken=default)
    {
        var filter = Builders<Report>.Filter.Eq(r => r.ReportType, reportType);
        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByFilterAsync(Expression<Func<Report, bool>> filter,CancellationToken cancellationToken=default)
    {
        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetPaginatedAsync(int pageNumber, int pageSize,CancellationToken cancellationToken=default)
    {
        var skip = (pageNumber - 1) * pageSize;
        var cursor = await _collection.Find(r => !r.IsDeleted)
            .Skip(skip)
            .Limit(pageSize)
            .ToCursorAsync();
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByDateRangeAsync(DateTime startDate, DateTime endDate,CancellationToken cancellationToken=default)
    {
        var filter = Builders<Report>.Filter.And(
            Builders<Report>.Filter.Gte(r => r.CreatedAt, startDate),
            Builders<Report>.Filter.Lte(r => r.CreatedAt, endDate),
            Builders<Report>.Filter.Eq(r => r.IsDeleted, false)
        );

        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }

    public async Task<IEnumerable<Report>> GetByDateSelectAsync(DateSelect dateSelect, CancellationToken cancellationToken = default)
    {
        var filter = Builders<Report>.Filter.Eq(r => r.DateSelect, dateSelect);
        var cursor = await _collection.FindAsync(filter);
        return await cursor.ToListAsync();
    }
}