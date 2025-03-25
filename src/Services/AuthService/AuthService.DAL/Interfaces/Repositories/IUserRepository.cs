using AuthService.Domain.Enities;

namespace AuthService.DAL.Interfaces.Repositories;

public interface IUserRepository:IBaseRepository<User>
{
    Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken=default);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken=default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task<User> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<User> GetByWarehouseIdAsync(Guid warehouseId, CancellationToken cancellationToken = default);
}