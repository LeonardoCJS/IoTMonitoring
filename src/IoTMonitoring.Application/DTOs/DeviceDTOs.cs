namespace IoTMonitoring.Application.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
    }

    public class CreateDeviceDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class SensorDataDto
    {
        public int Id { get; set; }
        public string DeviceId { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Unit { get; set; } = string.Empty;
    }

    public class CreateSensorDataDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}