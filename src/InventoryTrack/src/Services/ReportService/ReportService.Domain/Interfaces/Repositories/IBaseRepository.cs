using ReportService.Domain.Common;

namespace ReportService.Domain.Interfaces.Repositories;

public interface IBaseRepository<T> where T : BaseEntity
{
    IAsyncEnumerable<T> GetAllAsync();
    Task<T> GetByIdAsync(Guid id);
    Task CreateAsync(T entity);
    Task DeleteAsync(Guid id);
}