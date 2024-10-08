using AuthService.BLL.DTOs.Implementations.Requests.Company;
using AuthService.BLL.DTOs.Implementations.Responses.Company;
using AuthService.BLL.Interfaces.Services;

namespace AuthService.BLL.Services;

public class CompanyService:ICompanyService
{
    public Task<CompanyResponseDTO> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(CreateCompanyDTO companyDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UpdateCompanyDTO companyDto, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<CompanyResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}