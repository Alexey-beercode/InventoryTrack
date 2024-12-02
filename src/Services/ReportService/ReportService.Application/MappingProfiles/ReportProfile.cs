using AutoMapper;
using ReportService.Application.DTOs.Response.Report;
using ReportService.Domain.Entities;

namespace ReportService.Application.MappingProfiles;

public class ReportProfile : Profile
{
    public ReportProfile()
    {
        
        CreateMap<Report, ReportResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.ReportType))
            .ForMember(dest => dest.DateSelect, opt => opt.MapFrom(src => src.DateSelect))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}