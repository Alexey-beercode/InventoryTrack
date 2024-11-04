using AuthService.DAL.Infrastructure.Database;
using AuthService.DAL.Interfaces;
using AuthService.DAL.Interfaces.Repositories;

namespace AuthService.DAL.Infrastructure;

public class UnitOfWork:IUnitOfWork
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly AuthDbContext _dbContext;

    public UnitOfWork(IUserRepository userRepository, IRoleRepository roleRepository, ICompanyRepository companyRepository, AuthDbContext dbContext)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _companyRepository = companyRepository;
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public IUserRepository Users => _userRepository;

    public IRoleRepository Roles => _roleRepository;
    
    public ICompanyRepository Companies => _companyRepository;
}