namespace IoTMonitoring.Infrastructure.Repositories
{
    public class SensorDataRepository : ISensorDataRepository
    {
        private readonly ApplicationDbContext _context;

        public SensorDataRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SensorData?> GetByIdAsync(int id)
        {
            return await _context.SensorData
                .Include(sd => sd.Device)
                .FirstOrDefaultAsync(sd => sd.Id == id);
        }

        public async Task<IEnumerable<SensorData>> GetByDeviceIdAsync(string deviceId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.SensorData
                .Include(sd => sd.Device)
                .Where(sd => sd.DeviceId == deviceId);

            if (startDate.HasValue)
            {
                query = query.Where(sd => sd.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(sd => sd.Timestamp <= endDate.Value);
            }

            return await query
                .OrderByDescending(sd => sd.Timestamp)
                .ToListAsync();
        }

        public async Task AddAsync(SensorData sensorData)
        {
            await _context.SensorData.AddAsync(sensorData);
        }

        public async Task AddRangeAsync(IEnumerable<SensorData> sensorData)
        {
            await _context.SensorData.AddRangeAsync(sensorData);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}