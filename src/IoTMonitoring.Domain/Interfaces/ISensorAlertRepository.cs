using IoTMonitoring.Domain.Entities;

namespace IoTMonitoring.Domain.Interfaces
{
    public interface ISensorAlertRepository
    {
        Task<IEnumerable<SensorAlert>> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<SensorAlert>> GetUnacknowledgedAsync();
        Task AddAsync(SensorAlert alert);
        Task AcknowledgeAsync(string id);
    }
}
