namespace IoTMonitoring.Infrastructure.MongoDB
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string AlertsCollectionName { get; set; } = "SensorAlerts";
    }
}
