using Microsoft.Extensions.DependencyInjection;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Application.Common;

namespace IoTMonitoring.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<ISensorDataService, SensorDataService>();
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}
