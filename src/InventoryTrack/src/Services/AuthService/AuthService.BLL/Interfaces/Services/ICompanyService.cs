using AuthService.BLL.DTOs.Implementations.Requests.Company;
using AuthService.BLL.DTOs.Implementations.Responses.Company;

namespace AuthService.BLL.Interfaces.Services;

public interface ICompanyService
{
    Task<CompanyResponseDTO> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task CreateAsync(CreateCompanyDTO companyDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(UpdateCompanyDTO companyDto, CancellationToken cancellationToken = default);
    Task<List<CompanyResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid companyId, CancellationToken cancellationToken = default);
    Task<CompanyResponseDTO> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}