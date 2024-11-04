using AuthService.DAL.Interfaces.Repositories;

namespace AuthService.DAL.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    ICompanyRepository Companies { get; }
}