using AuthService.DAL.Infrastructure.Database;
using AuthService.DAL.Interfaces.Repositories;
using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Repositories;

public class RoleRepository:BaseRepository<Role>,IRoleRepository
{
    public RoleRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(Guid userId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.Roles
            .Where(role => _dbContext.UserRoles
                .Any(userRole => userRole.UserId == userId && !userRole.IsDeleted && role.Id==userRole.RoleId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckUserHasRoleAsync(Guid roleId, CancellationToken cancellationToken=default)
    {
        return await _dbContext.UserRoles.AnyAsync(ur => ur.RoleId == roleId && !ur.IsDeleted,cancellationToken);
    }

    public async Task<bool> SetRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, cancellationToken);
        var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted, cancellationToken);

        if (user == null || role == null)
        {
            return false;
        }

        var isExists =
            await _dbContext.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted, cancellationToken);
        if (isExists)
        {
            return false;
        }

        var userRole = new UserRole { UserId = userId, RoleId = roleId };
        await _dbContext.UserRoles.AddAsync(userRole, cancellationToken);
        return true;
    }

    public async Task<Role> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
    }

    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var userRole = await _dbContext.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && !ur.IsDeleted, cancellationToken);

        if (userRole == null)
        {
            return false;
        }

        userRole.IsDeleted = true;
        _dbContext.UserRoles.Update(userRole);
        return true;
    }

    public async Task DeleteAsync(Role role, CancellationToken cancellationToken = default)
    {
        role.IsDeleted = true;
        _dbSet.Update(role);
        
        await _dbContext.UserRoles
            .Where(userRole => userRole.RoleId == role.Id && !userRole.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(userRole => userRole.IsDeleted, true), cancellationToken);
    }
}