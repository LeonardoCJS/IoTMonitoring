using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;
using IoTMonitoring.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace IoTMonitoring.API.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services =>
        {
            // Substitui Oracle por banco em memória
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("IoTMonitoringTestDb");
            });

            // Substitui MongoDB por mock para não depender de instância local
            services.RemoveAll(typeof(ISensorAlertRepository));
            var mockAlertRepo = new Mock<ISensorAlertRepository>();
            mockAlertRepo.Setup(r => r.GetUnacknowledgedAsync())
                .ReturnsAsync(new List<SensorAlert>());
            mockAlertRepo.Setup(r => r.GetByDeviceIdAsync(It.IsAny<string>()))
                .ReturnsAsync(new List<SensorAlert>());
            mockAlertRepo.Setup(r => r.AddAsync(It.IsAny<SensorAlert>()))
                .Returns(Task.CompletedTask);
            mockAlertRepo.Setup(r => r.AcknowledgeAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);
            services.AddSingleton(mockAlertRepo.Object);
        });
    }
}
