namespace IoTMonitoring.Domain.Interfaces
{
    public interface ISensorDataRepository
    {
        Task<SensorData?> GetByIdAsync(int id);
        Task<IEnumerable<SensorData>> GetByDeviceIdAsync(string deviceId, DateTime? startDate = null, DateTime? endDate = null);
        Task AddAsync(SensorData sensorData);
        Task AddRangeAsync(IEnumerable<SensorData> sensorData);
        Task<bool> SaveAllAsync();
    }
}