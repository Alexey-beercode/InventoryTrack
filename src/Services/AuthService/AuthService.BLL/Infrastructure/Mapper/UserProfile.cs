using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.BLL.DTOs.Implementations.Requests.User;
using AuthService.BLL.DTOs.Implementations.Responses.User;
using AuthService.Domain.Enities;
using AutoMapper;

namespace AuthService.BLL.Infrastructure.Mapper;

public class UserProfile:Profile
{
    public UserProfile()
    {
        CreateMap<User, UserRepsonseDTO>();
        CreateMap<UpdateUserDTO, User>();
        CreateMap<RegisterUserToCompanyDTO, User>()
            .ForMember(dest => dest.CompanyId, 
                opt => opt.MapFrom(src => src.CompanyId));
    }
}