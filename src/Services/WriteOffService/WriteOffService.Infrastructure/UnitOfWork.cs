using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Domain.Interfaces.UnitOfWork;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private bool _disposed;
    private readonly WriteOffDbContext _dbContext;
    private readonly IDocumentRepository _documentRepository;
    private readonly IWriteOffActRepository _writeOffActRepository;
    private readonly IWriteOffReasonRepository _writeOffReasonRepository;
    private readonly IWriteOffRequestRepository _writeOffRequestRepository;

    public UnitOfWork(WriteOffDbContext dbContext, IDocumentRepository documentRepository, IWriteOffActRepository writeOffActRepository, IWriteOffReasonRepository writeOffReasonRepository, IWriteOffRequestRepository writeOffRequestRepository)
    {
        _dbContext = dbContext;
        _documentRepository = documentRepository;
        _writeOffActRepository = writeOffActRepository;
        _writeOffReasonRepository = writeOffReasonRepository;
        _writeOffRequestRepository = writeOffRequestRepository;
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

    public IDocumentRepository Documents => _documentRepository;
    public IWriteOffActRepository WriteOffActs => _writeOffActRepository;
    public IWriteOffReasonRepository WriteOffReasons => _writeOffReasonRepository;
    public IWriteOffRequestRepository WriteOffRequests => _writeOffRequestRepository;
}