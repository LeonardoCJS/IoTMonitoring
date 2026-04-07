using AutoMapper;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Web.ViewModels;

namespace IoTMonitoring.Web.Mapping;

public class WebMappingProfile : Profile
{
    public WebMappingProfile()
    {
        CreateMap<DeviceDto, DeviceViewModel>();
        CreateMap<DeviceDto, EditDeviceViewModel>();
        CreateMap<DeviceDto, DeleteDeviceViewModel>();
        CreateMap<CreateDeviceViewModel, CreateDeviceDto>();
        CreateMap<SensorDataDto, SensorDataViewModel>();
        CreateMap<CreateSensorDataViewModel, CreateSensorDataDto>();
    }
}
