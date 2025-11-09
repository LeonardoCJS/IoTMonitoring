using Microsoft.Extensions.DependencyInjection;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Application.Common;

namespace IoTMonitoring.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped();
            services.AddScoped();
            services.AddAutoMapper(typeof(MappingProfile));
            return services;
        }
    }
}
