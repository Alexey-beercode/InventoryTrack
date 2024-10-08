using AuthService.BLL.DTOs.Implementations.Responses.Role;
using AuthService.Domain.Enities;
using AutoMapper;

namespace AuthService.BLL.Infrastructure.Mapper;

public class RoleProfile:Profile
{
    public RoleProfile()
    {
        CreateMap<Role, RoleDTO>();
    }
}