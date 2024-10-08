using AuthService.DAL.Infrastructure.Database;
using AuthService.DAL.Interfaces.Repositories;
using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Repositories;

public class UserRepository:BaseRepository<User>,IUserRepository
{
    public UserRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<User> GetByLoginAsync(string login, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login && !u.IsDeleted, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(Guid roleId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Users
            .Where(user => _dbContext.UserRoles
                .Any(userRole => userRole.RoleId == roleId && !userRole.IsDeleted && user.Id==userRole.UserId))
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        user.IsDeleted = true;
        _dbSet.Update(user);
        
        await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == user.Id && !userRole.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(userRole => userRole.IsDeleted, true), cancellationToken);
    }

    public async Task<User> GetByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(
            user => !user.IsDeleted && user.FirstName == firstName && user.LastName == lastName, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(user => !user.IsDeleted && user.CompanyId == companyId).ToListAsync(cancellationToken);
    }
}