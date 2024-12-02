using MongoDB.Driver;
using ReportService.Domain.Common;
using ReportService.Domain.Interfaces.Repositories;
using ReportService.Infrastructure.Database;

namespace ReportService.Infrastructure.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _collection;

    public BaseRepository(ReportDbContext context, string collectionName)
    {
        _collection = context.GetCollection<T>(collectionName);
    }

    public async IAsyncEnumerable<T> GetAllAsync()
    {
        var cursor = await _collection.FindAsync(e => !e.IsDeleted);
        while (await cursor.MoveNextAsync())
        {
            foreach (var document in cursor.Current)
            {
                yield return document;
            }
        }
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _collection.Find(e => e.Id == id && !e.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(T entity)
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
        }

        await _collection.InsertOneAsync(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<T>.Filter.Eq(e => e.Id, id);
        var update = Builders<T>.Update
            .Set(e => e.IsDeleted, true);
        await _collection.UpdateOneAsync(filter, update);
    }
}