using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Entities;
using WriteOffService.Domain.Enums;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Domain.Models;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure.Repositories;

public class WriteOffRequestRepository : BaseRepository<WriteOffRequest> , IWriteOffRequestRepository
{
    public WriteOffRequestRepository(WriteOffDbContext dbContext) : base(dbContext)
    {
    }

    public new async Task<IEnumerable<WriteOffRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(request => request.Reason)
            .AsNoTracking().Where(request => !request.IsDeleted)
            .ToListAsync(cancellationToken);
    }
    
    public new async Task<WriteOffRequest> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(request => request.Reason)
            .AsNoTracking()
            .FirstOrDefaultAsync(request => request.Id == id && !request.IsDeleted, cancellationToken);
    }
    public async Task<IEnumerable<WriteOffRequest>> GetByApprovedUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Reason)
            .Where(request => request.ApprovedByUserId == userId && !request.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<WriteOffRequest> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(request => !request.IsDeleted && request.ItemId == itemId,cancellationToken);
    }
    
    public async Task<WriteOffRequest> GetByItemIdWithReasonAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Reason)
            .FirstOrDefaultAsync(request => !request.IsDeleted && request.ItemId == itemId, cancellationToken);
    }

    public async Task<WriteOffRequest> GetByDateAndItemIdAsync(Guid itemId, DateTime date, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Reason)
            .FirstOrDefaultAsync(request => !request.IsDeleted && request.ItemId == itemId && request.RequestDate==date, cancellationToken);
    }

    public async Task<IEnumerable<WriteOffRequest>> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Reason)
            .Where(request => request.WarehouseId == warehouseId && !request.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WriteOffRequest>> GetByStatusAsync(RequestStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(x => x.Reason)
            .Where(request => request.Status == status && !request.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WriteOffRequest>> GetFilteredAndPagedAsync(
        FilterWriteOffrequestModel filter, 
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(x => x.Reason)
            .Where(request => !request.IsDeleted);
        
        if (filter.ItemId != Guid.Empty)
            query = query.Where(r => r.ItemId == filter.ItemId);

        if (filter.WarehouseId != Guid.Empty)
            query = query.Where(r => r.WarehouseId == filter.WarehouseId);

        if (filter.Quantity > 0)
            query = query.Where(r => r.Quantity == filter.Quantity);

        if (filter.ReasonId != Guid.Empty)
            query = query.Where(r => r.ReasonId == filter.ReasonId);

        if (filter.Status != RequestStatus.None)
            query = query.Where(r => r.Status == filter.Status);

        if (filter.RequestDate != default)
            query = query.Where(r => r.RequestDate.Date == filter.RequestDate.Date);

        if (filter.ApprovedByUserId.HasValue)
            query = query.Where(r => r.ApprovedByUserId == filter.ApprovedByUserId);
        
        query = query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);
        
        return await query
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}