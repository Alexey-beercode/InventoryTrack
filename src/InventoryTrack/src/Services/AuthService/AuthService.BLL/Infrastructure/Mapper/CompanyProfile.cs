using AuthService.BLL.DTOs.Implementations.Requests.Company;
using AuthService.BLL.DTOs.Implementations.Responses.Company;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.Domain.Enities;
using AutoMapper;

namespace AuthService.BLL.Infrastructure.Mapper;

public class CompanyProfile:Profile
{
    public CompanyProfile()
    {
        CreateMap<Company, CompanyResponseDTO>();
        CreateMap<CreateCompanyDTO, Company>();
        CreateMap<UpdateCompanyDTO, Company>();
        CreateMap<Company, UserRepsonseDTO>();
    }
}