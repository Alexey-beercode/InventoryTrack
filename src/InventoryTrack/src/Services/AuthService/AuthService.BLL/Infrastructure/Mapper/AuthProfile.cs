using AutoMapper;
using AuthService.BLL.DTOs.Implementations.Requests.Auth;
using AuthService.Domain.Enities;

public class AuthProfile : Profile
{
    public AuthProfile()
    {
        CreateMap<RegisterDTO, User>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}