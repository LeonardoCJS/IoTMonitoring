
namespace IoTMonitoring.Domain.Entities
{
    public class Device : BaseEntity
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public string Location { get; set; } = string.Empty;
        public DateTime LastSeen { get; set; }
        public ICollection<SensorData> SensorData { get; set; } = new List<SensorData>();
    }

    public enum DeviceStatus
    {
        Online,
        Offline,
        Error
    }
}
