using WriteOffService.Domain.Interfaces.Repositories;

namespace WriteOffService.Domain.Interfaces.UnitOfWork;

public interface IUnitOfWork:IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken=default);
    IDocumentRepository Documents { get; }
    IWriteOffActRepository WriteOffActs { get; }
    IWriteOffReasonRepository WriteOffReasons { get; }
    IWriteOffRequestRepository WriteOffRequests { get; }
}