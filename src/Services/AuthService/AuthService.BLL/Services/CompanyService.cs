using AuthService.BLL.DTOs.Implementations.Requests.Company;
using AuthService.BLL.DTOs.Implementations.Responses.Company;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.BLL.Exceptions;
using AuthService.BLL.Interfaces.Services;
using AuthService.DAL.Interfaces;
using AuthService.Domain.Enities;
using AutoMapper;
using EventMaster.BLL.Exceptions;

namespace AuthService.BLL.Services;

public class CompanyService:ICompanyService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public CompanyService(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<DTOs.Implementations.Responses.Company.CompanyResponseDTO> GetByIdAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
        if (company==null)
        {
            throw new EntityNotFoundException("Company", companyId);
        }

        return _mapper.Map<DTOs.Implementations.Responses.Company.CompanyResponseDTO>(company);
    }

    public async Task CreateAsync(CreateCompanyDTO companyDto, CancellationToken cancellationToken = default)
    {
        var companyFromDb = await _unitOfWork.Companies.GetByUnpAsync(companyDto.Unp, cancellationToken);
        if (companyFromDb!=null)
        {
            throw new AlreadyExistsException("Company");
        }

        var responsibleUser = await _unitOfWork.Users.GetByIdAsync(companyDto.ResponsibleUserId, cancellationToken);
        if (responsibleUser == null)
        {
            throw new EntityNotFoundException("User", companyDto.ResponsibleUserId);
        }
        
        var company = _mapper.Map<Company>(companyDto);
        
        await _unitOfWork.Companies.CreateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var newCompany = await _unitOfWork.Companies.GetByUnpAsync(companyDto.Unp, cancellationToken);
        responsibleUser.CompanyId = newCompany.Id;
        
        _unitOfWork.Users.Update(responsibleUser);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(UpdateCompanyDTO companyDto, CancellationToken cancellationToken = default)
    {
        var companyFromDb = await _unitOfWork.Companies.GetByIdAsync(companyDto.Id, cancellationToken);
        if (companyFromDb==null)
        {
            throw new EntityNotFoundException("Company", companyDto.Id);
        }

        var company = _mapper.Map<Company>(companyDto);
        _unitOfWork.Companies.Update(company);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<CompanyResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var companies = await _unitOfWork.Companies.GetAllAsync(cancellationToken);
        return _mapper.Map<List<CompanyResponseDTO>>(companies);
    }

    public async Task DeleteAsync(Guid companyId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByIdAsync(companyId, cancellationToken);
        if (company==null)
        {
            throw new EntityNotFoundException("Company", companyId);
        }
        
        await _unitOfWork.Companies.DeleteAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CompanyResponseDTO> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var company = await _unitOfWork.Companies.GetByUserIdAsync(userId, cancellationToken);

        if (company is null)
        {
            throw new EntityNotFoundException("Company", userId);
        }

        return _mapper.Map<CompanyResponseDTO>(company);
    }
}