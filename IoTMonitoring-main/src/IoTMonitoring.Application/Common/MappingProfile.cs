using AutoMapper;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Web.ViewModels;

namespace IoTMonitoring.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Domain to DTO
            CreateMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            
            CreateMap();
            
            CreateMap()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => DeviceStatus.Offline))
                .ForMember(dest => dest.LastSeen, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            CreateMap()
                .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.UtcNow));

            // DTO to ViewModel
            CreateMap()
                .ForMember(dest => dest.RecentSensorData, opt => opt.Ignore());
            
            CreateMap()
                .ForMember(dest => dest.DeviceName, opt => opt.Ignore());

            CreateMap();
            CreateMap();

            // ViewModel to DTO
            CreateMap();
            CreateMap();
            
            CreateMap()
                .ReverseMap();
        }
    }
}
