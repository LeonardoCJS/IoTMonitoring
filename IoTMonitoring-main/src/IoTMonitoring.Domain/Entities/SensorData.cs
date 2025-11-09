namespace IoTMonitoring.Domain.Entities
{
    public class SensorData : BaseEntity
    {
        public string DeviceId { get; set; } = string.Empty;
        public string SensorType { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public DateTime Timestamp { get; set; }
        public string Unit { get; set; } = string.Empty;
        
        // Navigation property
        public Device Device { get; set; } = null!;
    }
}