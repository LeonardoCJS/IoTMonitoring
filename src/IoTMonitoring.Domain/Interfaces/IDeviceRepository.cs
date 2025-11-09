namespace IoTMonitoring.Domain.Interfaces
{
    public interface IDeviceRepository
    {
        Task<Device?> GetByIdAsync(int id);
        Task<Device?> GetByDeviceIdAsync(string deviceId);
        Task<IEnumerable<Device>> GetAllAsync();
        Task AddAsync(Device device);
        void Update(Device device);
        void Delete(Device device);
        Task<bool> SaveAllAsync();
    }
}
