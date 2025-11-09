namespace IoTMonitoring.Application.Services
{
    public interface IDeviceService
    {
        Task<DeviceDto?> GetDeviceByIdAsync(int id);
        Task<DeviceDto?> GetDeviceByDeviceIdAsync(string deviceId);
        Task<IEnumerable<DeviceDto>> GetAllDevicesAsync();
        Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto createDeviceDto);
        Task UpdateDeviceStatusAsync(string deviceId, DeviceStatus status);
        Task<bool> DeleteDeviceAsync(int id);
    }

    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public DeviceService(IDeviceRepository deviceRepository, IMapper mapper)
        {
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<DeviceDto?> GetDeviceByIdAsync(int id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<DeviceDto?> GetDeviceByDeviceIdAsync(string deviceId)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            return _mapper.Map<DeviceDto>(device);
        }

        public async Task<IEnumerable<DeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _deviceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DeviceDto>>(devices);
        }

        public async Task<DeviceDto> CreateDeviceAsync(CreateDeviceDto createDeviceDto)
        {
            var device = new Device
            {
                DeviceId = createDeviceDto.DeviceId,
                Name = createDeviceDto.Name,
                Location = createDeviceDto.Location,
                Status = DeviceStatus.Offline,
                LastSeen = DateTime.UtcNow
            };

            await _deviceRepository.AddAsync(device);
            await _deviceRepository.SaveAllAsync();

            return _mapper.Map<DeviceDto>(device);
        }

        public async Task UpdateDeviceStatusAsync(string deviceId, DeviceStatus status)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            if (device != null)
            {
                device.Status = status;
                device.LastSeen = DateTime.UtcNow;
                _deviceRepository.Update(device);
                await _deviceRepository.SaveAllAsync();
            }
        }

        public async Task<bool> DeleteDeviceAsync(int id)
        {
            var device = await _deviceRepository.GetByIdAsync(id);
            if (device == null) return false;

            _deviceRepository.Delete(device);
            return await _deviceRepository.SaveAllAsync();
        }
    }
}