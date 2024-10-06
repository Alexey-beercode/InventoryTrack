using AuthService.Domain.Enities;

namespace AuthService.DAL.Interfaces.Repositories;

public interface ICompanyRepository:IBaseRepository<Company>
{
    public Task<Company> GetByNameAsync(string companyName, CancellationToken cancellationToken = default);
    public Task<Company> GetByUnpAsync(int unp, CancellationToken cancellationToken = default);
    public Task DeleteAsync(Company company, CancellationToken cancellationToken = default);
}