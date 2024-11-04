using AuthService.DAL.Infrastructure.Database;
using AuthService.DAL.Interfaces.Repositories;
using AuthService.Domain.Enities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DAL.Repositories;

public class CompanyRepository:BaseRepository<Company>,ICompanyRepository
{
    public CompanyRepository(AuthDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Company> GetByNameAsync(string companyName, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(company => company.Name == companyName && !company.IsDeleted,cancellationToken);
    }
    
    public async Task<Company> GetByUnpAsync(int unp, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(company => company.Unp==unp && !company.IsDeleted,cancellationToken);
    }

    public async Task DeleteAsync(Company company, CancellationToken cancellationToken = default)
    {
        company.IsDeleted = true;
        _dbSet.Update(company);
        
        await _dbContext.Users
            .Where(user => user.Id == company.ResponsibleUserId && !user.IsDeleted)
            .ExecuteUpdateAsync(s => s.SetProperty(user => user.IsDeleted, true), cancellationToken);
    }
}