using Microsoft.EntityFrameworkCore;
using WriteOffService.Domain.Common;
using WriteOffService.Domain.Interfaces.Repositories;
using WriteOffService.Infrastructure.Config.Database;

namespace WriteOffService.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly WriteOffDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(WriteOffDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T> GetByIdAsync(Guid? id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }
}