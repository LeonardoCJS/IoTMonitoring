using AutoMapper;
using IoTMonitoring.Application.Common;
using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Application.Services;
using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;
using Moq;
using Xunit;

namespace IoTMonitoring.Application.UnitTests;

public class DeviceServiceTests
{
    private readonly IMapper _mapper;

    public DeviceServiceTests()
    {
        var cfg = new MapperConfiguration(c => c.AddProfile<MappingProfile>());
        _mapper = cfg.CreateMapper();
    }

    [Fact]
    public async Task CreateDeviceAsync_DadoPayloadValido_DeveCriarDispositivo()
    {
        // Arrange
        var repo = new Mock<IDeviceRepository>();
        repo.Setup(r => r.AddAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);
        repo.Setup(r => r.SaveAllAsync()).ReturnsAsync(true);
        var service = new DeviceService(repo.Object, _mapper);
        var dto = new CreateDeviceDto { DeviceId = "dev-001", Name = "Sensor Sala", Location = "Sala A" };

        // Act
        var result = await service.CreateDeviceAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("dev-001", result.DeviceId);
        Assert.Equal("Sensor Sala", result.Name);
        repo.Verify(r => r.AddAsync(It.IsAny<Device>()), Times.Once);
        repo.Verify(r => r.SaveAllAsync(), Times.Once);
    }
}
