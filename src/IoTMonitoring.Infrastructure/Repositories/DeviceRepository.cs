namespace IoTMonitoring.Infrastructure.Repositories
{
    public class DeviceRepository : IDeviceRepository
    {
        private readonly ApplicationDbContext _context;

        public DeviceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Device?> GetByIdAsync(int id)
        {
            return await _context.Devices
                .Include(d => d.SensorData.OrderByDescending(sd => sd.Timestamp).Take(10))
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Device?> GetByDeviceIdAsync(string deviceId)
        {
            return await _context.Devices
                .Include(d => d.SensorData.OrderByDescending(sd => sd.Timestamp).Take(10))
                .FirstOrDefaultAsync(d => d.DeviceId == deviceId);
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _context.Devices
                .Include(d => d.SensorData.OrderByDescending(sd => sd.Timestamp).Take(5))
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task AddAsync(Device device)
        {
            await _context.Devices.AddAsync(device);
        }

        public void Update(Device device)
        {
            _context.Devices.Update(device);
        }

        public void Delete(Device device)
        {
            _context.Devices.Remove(device);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}