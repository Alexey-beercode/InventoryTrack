using MovementService.Domain.Interfaces.Repositories;
using MovementService.Domain.Interfaces.UnitOfWork;
using MovementService.Infrastructure.Config.Database;

namespace MovementService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly MovementDbContext _dbContext;
    private readonly IMovementRequestRepository _movementRequestRepository;

    public UnitOfWork(MovementDbContext dbContext, IMovementRequestRepository movementRequestRepository)
    {
        _dbContext = dbContext;
        _movementRequestRepository = movementRequestRepository;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }

            _disposed = true;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IMovementRequestRepository MovementRequests => _movementRequestRepository;
}