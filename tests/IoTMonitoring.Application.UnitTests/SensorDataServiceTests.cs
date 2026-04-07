using AutoMapper;
using IoTMonitoring.Application.Common;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;
using Moq;
using Xunit;

namespace IoTMonitoring.Application.UnitTests;

public class SensorDataServiceTests
{
    private readonly IMapper _mapper;

    public SensorDataServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
        _mapper = cfg.CreateMapper();
    }

    [Fact]
    public async Task AddSensorDataAsync_DadoDispositivoInexistente_DeveLancarArgumentException()
    {
        // Arrange
        var sensorRepo = new Mock<ISensorDataRepository>();
        var deviceRepo = new Mock<IDeviceRepository>();
        deviceRepo.Setup(r => r.GetByDeviceIdAsync("nao-existe")).ReturnsAsync((Device?)null);
        var service = new SensorDataService(sensorRepo.Object, deviceRepo.Object, _mapper);
        var dto = new CreateSensorDataDto { DeviceId = "nao-existe", SensorType = "temp", Value = 25.1m, Unit = "C" };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddSensorDataAsync(dto));
        sensorRepo.Verify(r => r.AddAsync(It.IsAny<SensorData>()), Times.Never);
    }
}
