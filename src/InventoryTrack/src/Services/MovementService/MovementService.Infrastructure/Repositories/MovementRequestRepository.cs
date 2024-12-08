using Microsoft.EntityFrameworkCore;
using MovementService.Domain.Entities;
using MovementService.Domain.Enums;
using MovementService.Domain.Interfaces.Repositories;
using MovementService.Infrastructure.Config.Database;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MovementService.Infrastructure.Repositories
{
    public class MovementRequestRepository : BaseRepository<MovementRequest>, IMovementRequestRepository
    {
        public MovementRequestRepository(MovementDbContext dbContext) : base(dbContext)
        {
        }
        
        public async Task<IEnumerable<MovementRequest>> GetByItemIdAsync(Guid itemId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.ItemId == itemId)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<MovementRequest>> GetByApprovedUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.ApprovedByUserId == userId)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<MovementRequest>> GetBySourceWarehouseIdAsync(Guid sourceWarehouseId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.SourceWarehouseId == sourceWarehouseId)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<MovementRequest>> GetByDestinationWarehouseIdAsync(Guid destinationWarehouseId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.DestinationWarehouseId == destinationWarehouseId)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<IEnumerable<MovementRequest>> GetByStatusAsync(MovementRequestStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(m => !m.IsDeleted && m.Status == status)
                .ToListAsync(cancellationToken);
        }
    }
}