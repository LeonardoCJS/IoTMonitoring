using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;
using Moq;
using Xunit;

namespace IoTMonitoring.Application.UnitTests;

public class SensorAlertServiceTests
{
    [Fact]
    public async Task CreateAlertAsync_DadoPayloadValido_DevePersistirERetornarAlerta()
    {
        // Arrange
        var repo = new Mock<ISensorAlertRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<SensorAlert>()))
            .Callback<SensorAlert>(a => a.Id = "507f1f77bcf86cd799439011")
            .Returns(Task.CompletedTask);

        var service = new SensorAlertService(repo.Object);
        var dto = new CreateSensorAlertDto
        {
            DeviceId = "DEVICE-001",
            SensorType = "Temperature",
            Value = 95.5m,
            Unit = "°C",
            AlertLevel = "Critical",
            Message = "Temperatura acima de 90°C"
        };

        // Act
        var result = await service.CreateAlertAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("DEVICE-001", result.DeviceId);
        Assert.Equal("Critical", result.AlertLevel);
        Assert.False(result.Acknowledged);
        repo.Verify(r => r.AddAsync(It.IsAny<SensorAlert>()), Times.Once);
    }

    [Fact]
    public async Task CreateAlertAsync_AlertLevelInvalido_DeveLancarArgumentException()
    {
        // Arrange
        var repo = new Mock<ISensorAlertRepository>();
        var service = new SensorAlertService(repo.Object);
        var dto = new CreateSensorAlertDto
        {
            DeviceId = "DEVICE-001",
            SensorType = "Temperature",
            Value = 95.5m,
            Unit = "°C",
            AlertLevel = "INVALIDO",
            Message = "Alerta inválido"
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAlertAsync(dto));
        repo.Verify(r => r.AddAsync(It.IsAny<SensorAlert>()), Times.Never);
    }

    [Fact]
    public async Task CreateAlertAsync_DeviceIdVazio_DeveLancarArgumentException()
    {
        // Arrange
        var repo = new Mock<ISensorAlertRepository>();
        var service = new SensorAlertService(repo.Object);
        var dto = new CreateSensorAlertDto
        {
            DeviceId = "",
            SensorType = "Humidity",
            Value = 10m,
            Unit = "%",
            AlertLevel = "Warning",
            Message = "Umidade baixa"
        };

        // Act + Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAlertAsync(dto));
    }

    [Fact]
    public async Task GetUnacknowledgedAsync_QuandoExistemAlertas_DeveRetornarApenas_NaoReconhecidos()
    {
        // Arrange
        var alertsPendentes = new List<SensorAlert>
        {
            new() { Id = "1", DeviceId = "DEV-001", AlertLevel = "Warning", Acknowledged = false },
            new() { Id = "2", DeviceId = "DEV-002", AlertLevel = "Critical", Acknowledged = false }
        };

        var repo = new Mock<ISensorAlertRepository>();
        repo.Setup(r => r.GetUnacknowledgedAsync()).ReturnsAsync(alertsPendentes);

        var service = new SensorAlertService(repo.Object);

        // Act
        var result = (await service.GetUnacknowledgedAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, a => Assert.False(a.Acknowledged));
    }
}
