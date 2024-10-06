using AuthService.Domain.Enities;

namespace AuthService.DAL.Interfaces.Repositories;

public interface IUserRepository:IBaseRepository<User>
{
    Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken);
    Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
}