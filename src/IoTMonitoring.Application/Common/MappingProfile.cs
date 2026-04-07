using AutoMapper;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Domain.Entities;

namespace IoTMonitoring.Application.Common
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Device, DeviceDto>();
            CreateMap<CreateDeviceDto, Device>();
            CreateMap<SensorData, SensorDataDto>();
            CreateMap<CreateSensorDataDto, SensorData>();
        }
    }
}