using IoTMonitoring.Application.DTOs;
using IoTMonitoring.Domain.Entities;
using IoTMonitoring.Domain.Interfaces;

namespace IoTMonitoring.Application.Services
{
    public interface ISensorAlertService
    {
        Task<IEnumerable<SensorAlertDto>> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<SensorAlertDto>> GetUnacknowledgedAsync();
        Task<SensorAlertDto> CreateAlertAsync(CreateSensorAlertDto dto);
        Task AcknowledgeAlertAsync(string id);
    }

    public class SensorAlertService : ISensorAlertService
    {
        private readonly ISensorAlertRepository _repository;

        public SensorAlertService(ISensorAlertRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<SensorAlertDto>> GetByDeviceIdAsync(string deviceId)
        {
            var alerts = await _repository.GetByDeviceIdAsync(deviceId);
            return alerts.Select(MapToDto);
        }

        public async Task<IEnumerable<SensorAlertDto>> GetUnacknowledgedAsync()
        {
            var alerts = await _repository.GetUnacknowledgedAsync();
            return alerts.Select(MapToDto);
        }

        public async Task<SensorAlertDto> CreateAlertAsync(CreateSensorAlertDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DeviceId))
                throw new ArgumentException("DeviceId é obrigatório.");

            if (dto.AlertLevel != "Warning" && dto.AlertLevel != "Critical")
                throw new ArgumentException("AlertLevel deve ser 'Warning' ou 'Critical'.");

            var alert = new SensorAlert
            {
                DeviceId = dto.DeviceId,
                SensorType = dto.SensorType,
                Value = dto.Value,
                Unit = dto.Unit,
                AlertLevel = dto.AlertLevel,
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                Acknowledged = false
            };

            await _repository.AddAsync(alert);
            return MapToDto(alert);
        }

        public async Task AcknowledgeAlertAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID do alerta é obrigatório.");

            await _repository.AcknowledgeAsync(id);
        }

        private static SensorAlertDto MapToDto(SensorAlert alert) => new()
        {
            Id = alert.Id,
            DeviceId = alert.DeviceId,
            SensorType = alert.SensorType,
            Value = alert.Value,
            Unit = alert.Unit,
            AlertLevel = alert.AlertLevel,
            Message = alert.Message,
            CreatedAt = alert.CreatedAt,
            Acknowledged = alert.Acknowledged
        };
    }
}
