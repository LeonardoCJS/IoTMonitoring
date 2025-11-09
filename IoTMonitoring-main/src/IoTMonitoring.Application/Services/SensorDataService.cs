namespace IoTMonitoring.Application.Services
{
    public interface ISensorDataService
    {
        Task<SensorDataDto?> GetSensorDataByIdAsync(int id);
        Task<IEnumerable<SensorDataDto>> GetSensorDataByDeviceAsync(string deviceId, DateTime? startDate, DateTime? endDate);
        Task<SensorDataDto> AddSensorDataAsync(CreateSensorDataDto createSensorDataDto);
        Task AddBulkSensorDataAsync(IEnumerable<CreateSensorDataDto> sensorDataDtos);
    }

    public class SensorDataService : ISensorDataService
    {
        private readonly ISensorDataRepository _sensorDataRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly IMapper _mapper;

        public SensorDataService(ISensorDataRepository sensorDataRepository, IDeviceRepository deviceRepository, IMapper mapper)
        {
            _sensorDataRepository = sensorDataRepository;
            _deviceRepository = deviceRepository;
            _mapper = mapper;
        }

        public async Task<SensorDataDto?> GetSensorDataByIdAsync(int id)
        {
            var sensorData = await _sensorDataRepository.GetByIdAsync(id);
            return _mapper.Map<SensorDataDto>(sensorData);
        }

        public async Task<IEnumerable<SensorDataDto>> GetSensorDataByDeviceAsync(string deviceId, DateTime? startDate, DateTime? endDate)
        {
            var sensorData = await _sensorDataRepository.GetByDeviceIdAsync(deviceId, startDate, endDate);
            return _mapper.Map<IEnumerable<SensorDataDto>>(sensorData);
        }

        public async Task<SensorDataDto> AddSensorDataAsync(CreateSensorDataDto createSensorDataDto)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(createSensorDataDto.DeviceId);
            if (device == null)
            {
                throw new ArgumentException($"Device with ID {createSensorDataDto.DeviceId} not found");
            }

            var sensorData = new SensorData
            {
                DeviceId = createSensorDataDto.DeviceId,
                SensorType = createSensorDataDto.SensorType,
                Value = createSensorDataDto.Value,
                Unit = createSensorDataDto.Unit,
                Timestamp = DateTime.UtcNow
            };

            await _sensorDataRepository.AddAsync(sensorData);
            await _sensorDataRepository.SaveAllAsync();

            // Update device status and last seen
            await UpdateDeviceStatus(createSensorDataDto.DeviceId);

            return _mapper.Map<SensorDataDto>(sensorData);
        }

        public async Task AddBulkSensorDataAsync(IEnumerable<CreateSensorDataDto> sensorDataDtos)
        {
            var sensorDataList = new List<SensorData>();
            var deviceIds = sensorDataDtos.Select(d => d.DeviceId).Distinct();

            foreach (var deviceId in deviceIds)
            {
                var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
                if (device == null)
                {
                    throw new ArgumentException($"Device with ID {deviceId} not found");
                }
            }

            foreach (var dto in sensorDataDtos)
            {
                var sensorData = new SensorData
                {
                    DeviceId = dto.DeviceId,
                    SensorType = dto.SensorType,
                    Value = dto.Value,
                    Unit = dto.Unit,
                    Timestamp = DateTime.UtcNow
                };
                sensorDataList.Add(sensorData);
            }

            await _sensorDataRepository.AddRangeAsync(sensorDataList);
            await _sensorDataRepository.SaveAllAsync();

            // Update status for all devices
            foreach (var deviceId in deviceIds)
            {
                await UpdateDeviceStatus(deviceId);
            }
        }

        private async Task UpdateDeviceStatus(string deviceId)
        {
            var device = await _deviceRepository.GetByDeviceIdAsync(deviceId);
            if (device != null && device.Status != DeviceStatus.Online)
            {
                device.Status = DeviceStatus.Online;
                device.LastSeen = DateTime.UtcNow;
                _deviceRepository.Update(device);
                await _deviceRepository.SaveAllAsync();
            }
        }
    }
}