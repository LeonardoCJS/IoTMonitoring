using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using IoTMonitoring.Domain.Interfaces;
using IoTMonitoring.Infrastructure.Data;
using IoTMonitoring.Infrastructure.Repositories;

namespace IoTMonitoring.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseOracle(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ISensorDataRepository, SensorDataRepository>();

            return services;
        }
    }
}