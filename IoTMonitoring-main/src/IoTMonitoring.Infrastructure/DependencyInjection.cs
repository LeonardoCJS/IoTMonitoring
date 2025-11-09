namespace IoTMonitoring.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<ISensorDataRepository, SensorDataRepository>();

            return services;
        }
    }
}